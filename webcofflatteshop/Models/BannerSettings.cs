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
    public string Title { get; set; } = "Ưu đãi hôm nay";
    public string Description { get; set; } = "Cập nhật banner khuyến mãi để Admin theo dõi nhanh trong menu.";
    public string? LinkUrl { get; set; }
}
