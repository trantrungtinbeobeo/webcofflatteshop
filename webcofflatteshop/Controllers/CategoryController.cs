using Microsoft.AspNetCore.Mvc;
using webcofflatteshop.Models;
using webcofflatteshop.Repository;

namespace webcofflatteshop.Controllers;

public class CategoryController : Controller
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(string categoryName)
    {
        var name = categoryName?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["CategoryError"] = "Tên danh mục không được để trống.";
            return RedirectToProductAdd();
        }

        var alreadyExists = _categoryRepository.GetAllCategories()
            .Any(category => string.Equals(category.Name, name, StringComparison.OrdinalIgnoreCase));

        if (alreadyExists)
        {
            TempData["CategoryError"] = "Danh mục này đã tồn tại.";
            return RedirectToProductAdd();
        }

        _categoryRepository.AddCategory(new Category { Name = name });
        TempData["CategorySuccess"] = "Đã thêm danh mục mới.";
        return RedirectToProductAdd();
    }

    private RedirectToActionResult RedirectToProductAdd()
    {
        return RedirectToAction("Add", "Product");
    }
}
