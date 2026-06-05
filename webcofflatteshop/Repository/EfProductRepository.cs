using Microsoft.EntityFrameworkCore;
using webcofflatteshop.Data;
using webcofflatteshop.Models;

namespace webcofflatteshop.Repository;

public class EfProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public EfProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Product> GetAll() => _context.Products
        .AsNoTracking()
        .Include(product => product.Category)
        .OrderBy(product => product.Id)
        .ToList();

    public Product? GetById(int id) => _context.Products
        .Include(product => product.Category)
        .FirstOrDefault(product => product.Id == id);

    public void Add(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;
        _context.Products.Add(product);
        _context.SaveChanges();
    }

    public void Update(Product product)
    {
        var existingProduct = _context.Products.Find(product.Id);
        if (existingProduct is null) return;

        existingProduct.Name = product.Name;
        existingProduct.Price = product.Price;
        existingProduct.Description = product.Description;
        existingProduct.CategoryId = product.CategoryId;
        existingProduct.ImageUrl = product.ImageUrl ?? existingProduct.ImageUrl;
        existingProduct.IsAvailable = product.IsAvailable;
        existingProduct.IsFeatured = product.IsFeatured;
        existingProduct.Stock = product.Stock;
        existingProduct.UpdatedAt = DateTime.UtcNow;
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var product = _context.Products.Find(id);
        if (product is null) return;

        var orderItems = _context.OrderItems.Where(item => item.ProductId == id).ToList();
        foreach (var item in orderItems)
        {
            item.ProductId = null;
        }

        _context.Products.Remove(product);
        _context.SaveChanges();
    }
}
