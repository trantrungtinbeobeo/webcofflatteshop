using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webcofflatteshop.Authentication;
using webcofflatteshop.Data;

namespace webcofflatteshop.Controllers.Api;

[ApiController]
[Route("api/reports")]
[Authorize(AuthenticationSchemes = ApiKeyAuthenticationDefaults.AuthenticationScheme)]
public class ReportsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReportsApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ReportSummaryResponse>> GetSummary()
    {
        var completedOrders = _context.Orders
            .AsNoTracking()
            .Where(order => order.Status == "Completed");

        return Ok(new ReportSummaryResponse
        {
            ProductCount = await _context.Products.CountAsync(),
            AvailableProductCount = await _context.Products.CountAsync(product => product.IsAvailable),
            OrderCount = await _context.Orders.CountAsync(),
            PendingOrderCount = await _context.Orders.CountAsync(order => order.Status == "Pending"),
            Revenue = await completedOrders.SumAsync(order => (decimal?)order.TotalAmount) ?? 0m,
            LowStockProductCount = await _context.Products.CountAsync(product => product.Stock <= 10)
        });
    }

    [HttpGet("orders")]
    public async Task<ActionResult<IEnumerable<OrderReportResponse>>> GetOrders()
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .OrderByDescending(order => order.CreatedAt)
            .Take(100)
            .Select(order => new OrderReportResponse
            {
                Id = order.Id,
                CustomerEmail = order.CustomerEmail,
                CustomerName = _context.Users
                    .Where(user => user.Id == order.UserId)
                    .Select(user => user.FullName)
                    .FirstOrDefault() ?? order.CustomerEmail,
                CustomerRole = _context.UserRoles
                    .Where(userRole => userRole.UserId == order.UserId)
                    .Join(_context.Roles, userRole => userRole.RoleId, role => role.Id, (_, role) => role.Name)
                    .Any(roleName => roleName == "Admin") ? "Admin" : "User",
                TotalAmount = order.TotalAmount,
                FulfillmentMethod = order.FulfillmentMethod,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                ItemCount = order.Items.Sum(item => item.Quantity)
            })
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("revenue")]
    public async Task<ActionResult<IEnumerable<RevenueReportResponse>>> GetRevenue()
    {
        var fromDate = DateTime.UtcNow.Date.AddDays(-29);
        var revenue = await _context.Orders
            .AsNoTracking()
            .Where(order => order.CreatedAt >= fromDate && order.Status == "Completed")
            .GroupBy(order => order.CreatedAt.Date)
            .Select(group => new RevenueReportResponse
            {
                Date = group.Key,
                OrderCount = group.Count(),
                Revenue = group.Sum(order => order.TotalAmount)
            })
            .OrderBy(item => item.Date)
            .ToListAsync();

        return Ok(revenue);
    }
}

public class ReportSummaryResponse
{
    public int ProductCount { get; set; }
    public int AvailableProductCount { get; set; }
    public int OrderCount { get; set; }
    public int PendingOrderCount { get; set; }
    public decimal Revenue { get; set; }
    public int LowStockProductCount { get; set; }
}

public class OrderReportResponse
{
    public int Id { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerRole { get; set; } = "User";
    public decimal TotalAmount { get; set; }
    public string FulfillmentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int ItemCount { get; set; }
}

public class RevenueReportResponse
{
    public DateTime Date { get; set; }
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
}
