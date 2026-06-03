using Microsoft.EntityFrameworkCore;
using webcofflatteshop.Data;
using webcofflatteshop.Models;

namespace webcofflatteshop.Repository;

public class EfCategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public EfCategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Category> GetAllCategories() => _context.Categories
        .AsNoTracking()
        .OrderBy(category => category.Id)
        .ToList();

    public Category? GetById(int id) => _context.Categories
        .AsNoTracking()
        .Include(category => category.Products)
        .FirstOrDefault(category => category.Id == id);

    public void AddCategory(Category category)
    {
        _context.Categories.Add(category);
        _context.SaveChanges();
    }

    public void UpdateCategory(Category category)
    {
        var existingCategory = _context.Categories.Find(category.Id);
        if (existingCategory is null) return;

        existingCategory.Name = category.Name;
        _context.SaveChanges();
    }

    public void DeleteCategory(int id)
    {
        var category = _context.Categories.Find(id);
        if (category is null) return;

        _context.Categories.Remove(category);
        _context.SaveChanges();
    }
}
