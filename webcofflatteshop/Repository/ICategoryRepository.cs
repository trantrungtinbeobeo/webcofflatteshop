using webcofflatteshop.Models;

namespace webcofflatteshop.Repository;

public interface ICategoryRepository
{
    IEnumerable<Category> GetAllCategories();
    void AddCategory(Category category);
}
