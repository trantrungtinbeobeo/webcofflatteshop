namespace webcofflatteshop.Models;

public class AdminStatisticsViewModel
{
    public int TotalProducts { get; set; }
    public int AvailableProducts { get; set; }
    public int OutOfStockProducts { get; set; }
    public int FeaturedProducts { get; set; }
    public int TotalCategories { get; set; }
    public int HomeBannerCount { get; set; }
    public int UploadBannerCount { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public IEnumerable<Product> RecentProducts { get; set; } = [];
    public IEnumerable<CustomerPurchaseSummary> CustomerPurchases { get; set; } = [];
}

public class CustomerPurchaseSummary
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalSpent { get; set; }
}
