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

    public Category? GetById(int id) => _categoryList.FirstOrDefault(c => c.Id == id);

    public void AddCategory(Category category)
    {
        category.Id = _categoryList.Count == 0 ? 1 : _categoryList.Max(c => c.Id) + 1;
        _categoryList.Add(category);
    }

    public void UpdateCategory(Category category)
    {
        var index = _categoryList.FindIndex(c => c.Id == category.Id);
        if (index == -1) return;

        _categoryList[index].Name = category.Name;
    }

    public void DeleteCategory(int id)
    {
        var category = _categoryList.FirstOrDefault(c => c.Id == id);
        if (category is null) return;

        _categoryList.Remove(category);
    }
}
