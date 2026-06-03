using webcofflatteshop.Models;

namespace webcofflatteshop.Repository;

public interface ICategoryRepository
{
    IEnumerable<Category> GetAllCategories();
    Category? GetById(int id);
    void AddCategory(Category category);
    void UpdateCategory(Category category);
    void DeleteCategory(int id);
}
