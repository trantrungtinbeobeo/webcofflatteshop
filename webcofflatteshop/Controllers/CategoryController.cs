using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webcofflatteshop.Models;
using webcofflatteshop.Repository;

namespace webcofflatteshop.Controllers;

[Authorize(Roles = "Admin")]
public class CategoryController : Controller
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public IActionResult Index()
    {
        var categories = _categoryRepository.GetAllCategories();
        return View(categories);
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

        if (CategoryNameExists(name))
        {
            TempData["CategoryError"] = "Danh mục này đã tồn tại.";
            return RedirectToProductAdd();
        }

        _categoryRepository.AddCategory(new Category { Name = name });
        TempData["CategorySuccess"] = "Đã thêm danh mục mới.";
        return RedirectToProductAdd();
    }

    public IActionResult Update(int id)
    {
        var category = _categoryRepository.GetById(id);
        if (category is null) return NotFound();

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(Category category)
    {
        category.Name = category.Name?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(category.Name))
        {
            ModelState.AddModelError(nameof(Category.Name), "Tên danh mục không được để trống.");
        }

        if (!ModelState.IsValid)
        {
            return View(category);
        }

        if (CategoryNameExists(category.Name, category.Id))
        {
            ModelState.AddModelError(nameof(Category.Name), "Danh mục này đã tồn tại.");
            return View(category);
        }

        _categoryRepository.UpdateCategory(category);
        TempData["CategorySuccess"] = "Đã cập nhật danh mục.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var category = _categoryRepository.GetById(id);
        if (category is null) return NotFound();

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id, bool deleteProducts = false)
    {
        var category = _categoryRepository.GetById(id);
        if (category is null) return NotFound();

        if (category.Products.Any() && !deleteProducts)
        {
            TempData["CategoryError"] = "Danh mục đang có sản phẩm. Hãy tích chọn xóa kèm sản phẩm nếu muốn xóa toàn bộ.";
            return View("Delete", category);
        }

        if (deleteProducts)
        {
            _categoryRepository.DeleteCategoryWithProducts(id);
            TempData["CategorySuccess"] = "Đã xóa danh mục và tất cả sản phẩm thuộc danh mục.";
            return RedirectToAction(nameof(Index));
        }

        _categoryRepository.DeleteCategory(id);
        TempData["CategorySuccess"] = "Đã xóa danh mục.";
        return RedirectToAction(nameof(Index));
    }

    private bool CategoryNameExists(string name, int? currentCategoryId = null)
    {
        return _categoryRepository.GetAllCategories()
            .Any(category =>
                category.Id != currentCategoryId &&
                string.Equals(category.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    private RedirectToActionResult RedirectToProductAdd()
    {
        return RedirectToAction("Add", "Product");
    }
}
