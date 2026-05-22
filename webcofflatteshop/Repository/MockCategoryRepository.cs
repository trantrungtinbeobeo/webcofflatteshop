using webcofflatteshop.Models;

namespace webcofflatteshop.Repository;

public class MockCategoryRepository : ICategoryRepository
{
    private readonly List<Category> _categoryList =
    [
        new() { Id = 1, Name = "Coffee" },
        new() { Id = 2, Name = "Non-Coffee" },
        new() { Id = 3, Name = "Bakery" }
    ];

    public IEnumerable<Category> GetAllCategories() => _categoryList;
}
