namespace webcofflatteshop.Models;

public class BannerSettings
{
    public List<string> HomeBanners { get; set; } = new();
    public List<string> UploadBanners { get; set; } = new();
    public PromoBannerSettings PromoBanner { get; set; } = new();
}

public class PromoBannerSettings
{
    public bool IsEnabled { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<PromoBannerItem> Items { get; set; } = new();
    public int? ProductId { get; set; }
    public int DiscountPercent { get; set; }
    public string Title { get; set; } = "Ưu đãi hôm nay";
    public string Description { get; set; } = "Theo dõi ưu đãi mới nhất tại Coffe Latte Kawaii.";
    public string? LinkUrl { get; set; }
}

public class PromoBannerItem
{
    public string ImageUrl { get; set; } = string.Empty;
    public int? ProductId { get; set; }
    public int DiscountPercent { get; set; }
    public string Title { get; set; } = "Ưu đãi hôm nay";
    public string Description { get; set; } = "Theo dõi ưu đãi mới nhất tại Coffe Latte Kawaii.";
    public string? LinkUrl { get; set; }
}
