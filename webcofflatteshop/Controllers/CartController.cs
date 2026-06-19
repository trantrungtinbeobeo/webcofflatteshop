using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webcofflatteshop.Data;
using webcofflatteshop.Models;
using webcofflatteshop.Repository;

namespace webcofflatteshop.Controllers;

[Authorize]
[Route("[controller]/[action]")]
public class CartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IBannerRepository _bannerRepository;

    public CartController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IBannerRepository bannerRepository)
    {
        _context = context;
        _userManager = userManager;
        _bannerRepository = bannerRepository;
    }

    [HttpPost]
    [HttpPost("/Product/Checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        if (request == null || request.Items == null || !request.Items.Any())
        {
            return BadRequest(new { success = false, message = "Giỏ hàng của bạn đang trống." });
        }

        var fulfillmentMethod = request.FulfillmentMethod == "Delivery" ? "Delivery" : "Pickup";
        var shippingFee = fulfillmentMethod == "Delivery" ? 15000m : 0m;

        var user = await _userManager.GetUserAsync(User);
        if (user is null || string.IsNullOrWhiteSpace(user.Email))
        {
            return Unauthorized(new { success = false, message = "Vui lòng đăng nhập trước khi đặt hàng." });
        }

        var productIds = request.Items.Select(item => item.ProductId).Distinct().ToList();
        var products = await _context.Products
            .Where(product => productIds.Contains(product.Id))
            .ToDictionaryAsync(product => product.Id);
        var promoBanner = _bannerRepository.Get().PromoBanner;
        var order = new Order
        {
            UserId = user.Id,
            CustomerEmail = user.Email,
            FulfillmentMethod = fulfillmentMethod,
            ShippingFee = shippingFee,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        foreach (var item in request.Items)
        {
            if (!products.TryGetValue(item.ProductId, out var product))
            {
                return BadRequest(new { success = false, message = "Sản phẩm trong giỏ hàng không còn tồn tại." });
            }

            if (item.Qty <= 0)
            {
                return BadRequest(new { success = false, message = "Số lượng sản phẩm không hợp lệ." });
            }

            if (product.Stock < item.Qty)
            {
                return BadRequest(new { success = false, message = $"Sản phẩm '{product.Name}' chỉ còn {product.Stock} ly trong kho." });
            }

            var unitPrice = ApplyPromoPrice(ApplySizePrice(product.Price, item.Size), product.Id, promoBanner);
            var lineTotal = unitPrice * item.Qty;
            product.Stock -= item.Qty;
            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = item.Qty,
                UnitPrice = unitPrice,
                LineTotal = lineTotal,
                Sugar = item.Sugar,
                Size = item.Size
            });
            order.SubtotalAmount += lineTotal;
        }

        order.TotalAmount = order.SubtotalAmount + order.ShippingFee;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            success = true,
            orderId = order.Id,
            message = $"Đặt hàng thành công! Mã đơn hàng của bạn là #DH{order.Id:00000}."
        });
    }

    private static decimal ApplySizePrice(decimal basePrice, string? size)
    {
        return size switch
        {
            "Nhỏ" => Math.Max(basePrice - 5000m, 0m),
            "Lớn" => basePrice + 10000m,
            _ => basePrice
        };
    }

    private static decimal ApplyPromoPrice(decimal price, int productId, PromoBannerSettings promoBanner)
    {
        if (!promoBanner.IsEnabled || promoBanner.ProductId != productId || promoBanner.DiscountPercent <= 0)
        {
            return price;
        }

        var discountPercent = Math.Clamp(promoBanner.DiscountPercent, 0, 100);
        return Math.Max(decimal.Round(price * (100 - discountPercent) / 100, 0), 0);
    }
}

public class CheckoutRequest
{
    public List<CartItemDto> Items { get; set; } = [];
    public string FulfillmentMethod { get; set; } = "Pickup";
}

public class CartItemDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Qty { get; set; }
    public string Sugar { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
}
