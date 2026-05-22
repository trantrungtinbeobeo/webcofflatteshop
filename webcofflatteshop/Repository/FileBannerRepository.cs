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
                UploadBanners = ["/images/default-coffee.svg", "/images/default-coffee.svg"]
            };
            Save(defaults);
        }
    }

    public BannerSettings Get()
    {
        lock (_lock)
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<BannerSettings>(json) ?? new BannerSettings();
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
