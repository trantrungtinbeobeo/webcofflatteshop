using webcofflatteshop.Models;

namespace webcofflatteshop.Repository;

public class MockCategoryRepository : ICategoryRepository
{
    private readonly List<Category> _categoryList =
    [
        new() { Id = 1, Name = "Coffee" },
        new() { Id = 2, Name = "Matcha" },
        new() { Id = 3, Name = "Chocolate" },
        new() { Id = 4, Name = "Bakery" }
    ];

    public IEnumerable<Category> GetAllCategories() => _categoryList.OrderBy(c => c.Id);

    public void AddCategory(Category category)
    {
        category.Id = _categoryList.Count == 0 ? 1 : _categoryList.Max(c => c.Id) + 1;
        _categoryList.Add(category);
    }
}
