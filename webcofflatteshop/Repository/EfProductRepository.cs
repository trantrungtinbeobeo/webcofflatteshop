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
        _context.Products.Add(product);
        _context.SaveChanges();
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var product = _context.Products.Find(id);
        if (product is null) return;

        _context.Products.Remove(product);
        _context.SaveChanges();
    }
}
