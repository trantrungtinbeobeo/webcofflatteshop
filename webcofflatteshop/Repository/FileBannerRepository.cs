using System.Text.Json;
using webcofflatteshop.Models;

namespace webcofflatteshop.Repository;

public class FileBannerRepository : IBannerRepository
{
    private readonly string _filePath;
    private readonly object _lock = new();

    public FileBannerRepository(IWebHostEnvironment env)
    {
        var dataDir = Path.Combine(env.ContentRootPath, "App_Data");
        Directory.CreateDirectory(dataDir);
        _filePath = Path.Combine(dataDir, "banners.json");

        if (!File.Exists(_filePath))
        {
            var defaults = new BannerSettings
            {
                HomeBanners = ["/images/default-coffee.svg", "/images/default-coffee.svg", "/images/default-coffee.svg"],
                UploadBanners = ["/images/default-coffee.svg", "/images/default-coffee.svg"],
                PromoBanner = new PromoBannerSettings()
            };
            Save(defaults);
        }
    }

    public BannerSettings Get()
    {
        lock (_lock)
        {
            var json = File.ReadAllText(_filePath);
            var settings = JsonSerializer.Deserialize<BannerSettings>(json) ?? new BannerSettings();
            settings.PromoBanner ??= new PromoBannerSettings();
            settings.PromoBanner.ImageUrls ??= new List<string>();
            settings.PromoBanner.Items ??= new List<PromoBannerItem>();
            if (!string.IsNullOrWhiteSpace(settings.PromoBanner.ImageUrl) && !settings.PromoBanner.ImageUrls.Contains(settings.PromoBanner.ImageUrl))
            {
                settings.PromoBanner.ImageUrls.Insert(0, settings.PromoBanner.ImageUrl);
            }
            foreach (var image in settings.PromoBanner.ImageUrls.Where(image => !string.IsNullOrWhiteSpace(image)).ToList())
            {
                if (settings.PromoBanner.Items.Any(item => item.ImageUrl == image)) continue;

                settings.PromoBanner.Items.Add(new PromoBannerItem
                {
                    ImageUrl = image,
                    ProductId = image == settings.PromoBanner.ImageUrl ? settings.PromoBanner.ProductId : null,
                    DiscountPercent = image == settings.PromoBanner.ImageUrl ? settings.PromoBanner.DiscountPercent : 0,
                    Title = settings.PromoBanner.Title,
                    Description = settings.PromoBanner.Description,
                    LinkUrl = settings.PromoBanner.LinkUrl
                });
            }
            return settings;
        }
    }

    public void Save(BannerSettings settings)
    {
        lock (_lock)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}
