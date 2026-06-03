using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webcofflatteshop.Data;
using webcofflatteshop.Models;
using webcofflatteshop.Repository;

namespace webcofflatteshop.Controllers;

public class ProductController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IBannerRepository _bannerRepository;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProductController(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IWebHostEnvironment webHostEnvironment,
        IBannerRepository bannerRepository,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _webHostEnvironment = webHostEnvironment;
        _bannerRepository = bannerRepository;
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        var products = _productRepository.GetAll();
        ViewBag.Banners = _bannerRepository.Get().HomeBanners;
        return View(products);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Add()
    {
        LoadAdminProductFormData();
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Add(Product product)
    {
        if (!ModelState.IsValid)
        {
            LoadAdminProductFormData();
            return View(product);
        }

        _productRepository.Add(product);
        TempData["ProductSuccess"] = "Đã thêm sản phẩm mới.";
        return RedirectToAction(nameof(Add));
    }

    public IActionResult Display(int id)
    {
        var product = _productRepository.GetById(id);
        if (product is null) return NotFound();
        return View(product);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Update(int id)
    {
        var product = _productRepository.GetById(id);
        if (product is null) return NotFound();
        LoadCategories();
        return View(product);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Update(Product product)
    {
        if (!ModelState.IsValid)
        {
            LoadCategories();
            return View(product);
        }

        _productRepository.Update(product);
        TempData["ProductSuccess"] = "Đã cập nhật sản phẩm.";
        return RedirectToAction(nameof(Add));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var product = _productRepository.GetById(id);
        if (product is null) return NotFound();
        return View(product);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        _productRepository.Delete(id);
        TempData["ProductSuccess"] = "Đã xóa sản phẩm.";
        return RedirectToAction(nameof(Add));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult UploadImageList()
    {
        var products = _productRepository.GetAll();
        var bannerSettings = _bannerRepository.Get();
        ViewBag.UploadBanners = bannerSettings.UploadBanners;
        ViewBag.HomeBanners = bannerSettings.HomeBanners;
        return View(products);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Statistics()
    {
        var products = _productRepository.GetAll().ToList();
        var bannerSettings = _bannerRepository.Get();
        var customerPurchases = _context.Orders
            .AsNoTracking()
            .GroupBy(order => order.UserId)
            .Select(group => new
            {
                UserId = group.Key,
                OrderCount = group.Count(),
                TotalSpent = group.Sum(order => order.TotalAmount)
            })
            .ToList()
            .Join(
                _context.Users.AsNoTracking().ToList(),
                purchase => purchase.UserId,
                user => user.Id,
                (purchase, user) => new CustomerPurchaseSummary
                {
                    UserId = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    Address = user.Address,
                    OrderCount = purchase.OrderCount,
                    TotalSpent = purchase.TotalSpent
                })
            .OrderByDescending(item => item.TotalSpent)
            .ToList();
        var model = new AdminStatisticsViewModel
        {
            TotalProducts = products.Count,
            AvailableProducts = products.Count(product => product.IsAvailable),
            OutOfStockProducts = products.Count(product => product.Stock <= 0),
            FeaturedProducts = products.Count(product => product.IsFeatured),
            TotalCategories = products
                .Where(product => product.Category is not null)
                .Select(product => product.CategoryId)
                .Distinct()
                .Count(),
            HomeBannerCount = bannerSettings.HomeBanners.Count,
            UploadBannerCount = bannerSettings.UploadBanners.Count,
            TotalInventoryValue = products.Sum(product => product.Price * product.Stock),
            RecentProducts = products.OrderByDescending(product => product.UpdatedAt).Take(8).ToList(),
            CustomerPurchases = customerPurchases
        };

        return View(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> UploadBannerImage(IFormFile bannerFile, string target = "home")
    {
        if (bannerFile is null || bannerFile.Length == 0)
        {
            TempData["BannerError"] = "Vui lòng chọn file banner.";
            return RedirectToAction(nameof(UploadImageList));
        }

        var extension = Path.GetExtension(bannerFile.FileName).ToLowerInvariant();
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".svg" };
        if (!allowed.Contains(extension))
        {
            TempData["BannerError"] = "Banner chỉ hỗ trợ JPG, PNG, WEBP, SVG.";
            return RedirectToAction(nameof(UploadImageList));
        }

        var bannerPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "banners");
        Directory.CreateDirectory(bannerPath);

        var fileName = $"banner-{target}-{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(bannerPath, fileName);

        await using (var stream = System.IO.File.Create(fullPath))
        {
            await bannerFile.CopyToAsync(stream);
        }

        var relative = $"/uploads/banners/{fileName}";
        var settings = _bannerRepository.Get();
        if (target == "upload")
        {
            settings.UploadBanners.Insert(0, relative);
            settings.UploadBanners = settings.UploadBanners.Take(5).ToList();
        }
        else
        {
            settings.HomeBanners.Insert(0, relative);
            settings.HomeBanners = settings.HomeBanners.Take(5).ToList();
        }

        _bannerRepository.Save(settings);
        TempData["BannerSuccess"] = "Upload banner thành công.";
        return RedirectToAction(nameof(UploadImageList));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult DeleteBanner(string bannerUrl, string target = "home")
    {
        var settings = _bannerRepository.Get();
        if (target == "upload") settings.UploadBanners.Remove(bannerUrl);
        else settings.HomeBanners.Remove(bannerUrl);
        _bannerRepository.Save(settings);
        TempData["BannerSuccess"] = "Đã xóa banner.";
        return RedirectToAction(nameof(UploadImageList));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult UploadImage(int id)
    {
        var product = _productRepository.GetById(id);
        if (product is null) return NotFound();
        return View(product);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult RemoveProductImage(int id)
    {
        var product = _productRepository.GetById(id);
        if (product is null) return NotFound();
        product.ImageUrl = null;
        _productRepository.Update(product);
        TempData["SuccessMessage"] = "Đã xóa ảnh sản phẩm.";
        return RedirectToAction(nameof(UploadImage), new { id });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> UploadImage(int id, IFormFile imageFile)
    {
        var product = _productRepository.GetById(id);
        if (product is null) return NotFound();

        if (imageFile is null || imageFile.Length == 0)
        {
            ModelState.AddModelError(string.Empty, "Vui lòng chọn file ảnh.");
            return View(product);
        }

        var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        if (!allowed.Contains(extension))
        {
            ModelState.AddModelError(string.Empty, "Chỉ hỗ trợ JPG, PNG, WEBP.");
            return View(product);
        }

        var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "products");
        Directory.CreateDirectory(uploadsPath);

        var fileName = $"product-{id}-{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(uploadsPath, fileName);

        await using (var stream = System.IO.File.Create(fullPath))
        {
            await imageFile.CopyToAsync(stream);
        }

        product.ImageUrl = $"/uploads/products/{fileName}";
        _productRepository.Update(product);

        TempData["SuccessMessage"] = "Tải ảnh sản phẩm thành công.";
        return RedirectToAction(nameof(UploadImage), new { id });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        if (request == null || request.Items == null || !request.Items.Any())
        {
            return BadRequest(new { success = false, message = "Giỏ hàng của bạn đang trống." });
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null || string.IsNullOrWhiteSpace(user.Email))
        {
            return Unauthorized(new { success = false, message = "Vui lòng đăng nhập trước khi đặt hàng." });
        }

        var productIds = request.Items.Select(item => item.ProductId).Distinct().ToList();
        var products = await _context.Products
            .Where(product => productIds.Contains(product.Id))
            .ToDictionaryAsync(product => product.Id);
        var order = new Order
        {
            UserId = user.Id,
            CustomerEmail = user.Email,
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

            var unitPrice = ApplySizePrice(product.Price, item.Size);
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
            order.TotalAmount += lineTotal;
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = $"Đặt hàng thành công! Mã đơn hàng của bạn là #{order.Id}." });
    }

    private void LoadCategories()
    {
        var categories = _categoryRepository.GetAllCategories();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
    }

    private void LoadAdminProductFormData()
    {
        LoadCategories();
        ViewBag.Products = _productRepository.GetAll()
            .OrderByDescending(product => product.UpdatedAt)
            .ThenByDescending(product => product.Id)
            .ToList();
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
}

public class CheckoutRequest
{
    public List<CartItemDto> Items { get; set; } = [];
}

public class CartItemDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Qty { get; set; }
    public string Sugar { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
}
