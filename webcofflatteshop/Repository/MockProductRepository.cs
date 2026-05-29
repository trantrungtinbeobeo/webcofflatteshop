using System.Text.Json;
using webcofflatteshop.Models;

namespace webcofflatteshop.Repository;

public class MockProductRepository : IProductRepository
{
    private readonly string _filePath;
    private readonly object _lock = new();
    private List<Product> _products;

    public MockProductRepository(IWebHostEnvironment env)
    {
        var dataDir = Path.Combine(env.ContentRootPath, "App_Data");
        Directory.CreateDirectory(dataDir);
        _filePath = Path.Combine(dataDir, "products.json");

        if (!File.Exists(_filePath))
        {
            _products = SeedProducts();
            Persist();
        }
        else
        {
            var json = File.ReadAllText(_filePath);
            _products = JsonSerializer.Deserialize<List<Product>>(json) ?? SeedProducts();
        }
    }

    public IEnumerable<Product> GetAll() => _products.OrderBy(p => p.Id);

    public Product? GetById(int id) => _products.FirstOrDefault(p => p.Id == id);

    public void Add(Product product)
    {
        lock (_lock)
        {
            product.Id = _products.Count == 0 ? 1 : _products.Max(p => p.Id) + 1;
            _products.Add(product);
            Persist();
        }
    }

    public void Update(Product product)
    {
        lock (_lock)
        {
            var index = _products.FindIndex(p => p.Id == product.Id);
            if (index != -1)
            {
                _products[index] = product;
                Persist();
            }
        }
    }

    public void Delete(int id)
    {
        lock (_lock)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product is not null)
            {
                _products.Remove(product);
                Persist();
            }
        }
    }

    private void Persist()
    {
        var json = JsonSerializer.Serialize(_products, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }

    private static List<Product> SeedProducts() =>
    [
        new() { Id = 1, Name = "Espresso", Price = 2.50m, Description = "Đậm đà vị cà phê Ý nguyên bản.", CategoryId = 1 },
        new() { Id = 2, Name = "Americano", Price = 3.00m, Description = "Espresso pha loãng, nhẹ nhàng và thơm.", CategoryId = 1 },
        new() { Id = 3, Name = "Cappuccino", Price = 3.80m, Description = "Bọt sữa mịn, cân bằng giữa sữa và cà phê.", CategoryId = 1 },
        new() { Id = 4, Name = "Latte", Price = 4.20m, Description = "Sữa béo mượt cùng espresso dịu êm.", CategoryId = 1 },
        new() { Id = 5, Name = "Mocha", Price = 4.50m, Description = "Hòa quyện cà phê và chocolate ngọt ngào.", CategoryId = 1 },
        new() { Id = 6, Name = "Caramel Macchiato", Price = 4.90m, Description = "Vị caramel thơm, hậu vị espresso mạnh.", CategoryId = 1 },
        new() { Id = 7, Name = "Cold Brew", Price = 4.30m, Description = "Ủ lạnh 18 tiếng, mượt và ít chua.", CategoryId = 1 },
        new() { Id = 8, Name = "Vietnamese Iced Coffee", Price = 3.70m, Description = "Cà phê sữa đá đậm vị Việt Nam.", CategoryId = 1 },
        new() { Id = 9, Name = "Matcha Latte", Price = 4.60m, Description = "Trà xanh Nhật kết hợp sữa thanh dịu.", CategoryId = 2 },
        new() { Id = 10, Name = "Chocolate Frappe", Price = 5.20m, Description = "Đá xay chocolate mát lạnh cho ngày hè.", CategoryId = 3 },
        new() { Id = 11, Name = "Croissant Butter", Price = 2.90m, Description = "Bánh sừng bò bơ giòn tan mỗi sáng.", CategoryId = 4 },
        new() { Id = 12, Name = "Tiramisu", Price = 4.10m, Description = "Bánh tiramisu mềm mịn thơm cà phê.", CategoryId = 4 },
        new() { Id = 13, Name = "Blueberry Cheesecake", Price = 4.80m, Description = "Cheesecake béo nhẹ phủ mứt việt quất.", CategoryId = 4 }
    ];
}
