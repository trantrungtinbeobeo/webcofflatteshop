using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using webcofflatteshop.Models;
using webcofflatteshop.Repository;

namespace webcofflatteshop.Controllers;

public class ProductController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IBannerRepository _bannerRepository;

    public ProductController(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IWebHostEnvironment webHostEnvironment,
        IBannerRepository bannerRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _webHostEnvironment = webHostEnvironment;
        _bannerRepository = bannerRepository;
    }

    public IActionResult Index()
    {
        var products = _productRepository.GetAll();
        ViewBag.Banners = _bannerRepository.Get().HomeBanners;
        return View(products);
    }

    public IActionResult Add()
    {
        LoadCategories();
        return View();
    }

    [HttpPost]
    public IActionResult Add(Product product)
    {
        if (!ModelState.IsValid)
        {
            LoadCategories();
            return View(product);
        }

        _productRepository.Add(product);
        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    public IActionResult AddCategory(string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            TempData["CategoryError"] = "Tên danh mục không được để trống.";
            return RedirectToAction(nameof(Add));
        }

        _categoryRepository.AddCategory(new Category { Name = categoryName.Trim() });
        TempData["CategorySuccess"] = "Đã thêm danh mục mới.";
        return RedirectToAction(nameof(Add));
    }

    public IActionResult Display(int id)
    {
        var product = _productRepository.GetById(id);
        if (product is null) return NotFound();
        return View(product);
    }

    public IActionResult Update(int id)
    {
        var product = _productRepository.GetById(id);
        if (product is null) return NotFound();
        LoadCategories();
        return View(product);
    }

    [HttpPost]
    public IActionResult Update(Product product)
    {
        if (!ModelState.IsValid)
        {
            LoadCategories();
            return View(product);
        }

        _productRepository.Update(product);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var product = _productRepository.GetById(id);
        if (product is null) return NotFound();
        return View(product);
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        _productRepository.Delete(id);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult UploadImageList()
    {
        var products = _productRepository.GetAll();
        var bannerSettings = _bannerRepository.Get();
        ViewBag.UploadBanners = bannerSettings.UploadBanners;
        ViewBag.HomeBanners = bannerSettings.HomeBanners;
        return View(products);
    }

    [HttpPost]
    public async Task<IActionResult> UploadBannerImage(IFormFile bannerFile, string target = "home")
    {
        if (bannerFile is null || bannerFile.Length == 0)
        {
            TempData["BannerError"] = "Vui lòng chọn file banner.";
            return RedirectToAction(nameof(UploadImageList));
        }

        var extension = Path.GetExtension(bannerFile.FileName).ToLowerInvariant();
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".svg" };
        if (!allowed.Contains(extension))
        {
            TempData["BannerError"] = "Banner chỉ hỗ trợ JPG, PNG, WEBP, SVG.";
            return RedirectToAction(nameof(UploadImageList));
        }

        var bannerPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "banners");
        Directory.CreateDirectory(bannerPath);

        var fileName = $"banner-{target}-{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(bannerPath, fileName);

        await using (var stream = System.IO.File.Create(fullPath))
        {
            await bannerFile.CopyToAsync(stream);
        }

        var relative = $"/uploads/banners/{fileName}";
        var settings = _bannerRepository.Get();
        if (target == "upload")
        {
            settings.UploadBanners.Insert(0, relative);
            settings.UploadBanners = settings.UploadBanners.Take(5).ToList();
        }
        else
        {
            settings.HomeBanners.Insert(0, relative);
            settings.HomeBanners = settings.HomeBanners.Take(5).ToList();
        }

        _bannerRepository.Save(settings);
        TempData["BannerSuccess"] = "Upload banner thành công.";
        return RedirectToAction(nameof(UploadImageList));
    }

    [HttpPost]
    public IActionResult DeleteBanner(string bannerUrl, string target = "home")
    {
        var settings = _bannerRepository.Get();
        if (target == "upload") settings.UploadBanners.Remove(bannerUrl);
        else settings.HomeBanners.Remove(bannerUrl);
        _bannerRepository.Save(settings);
        TempData["BannerSuccess"] = "Đã xóa banner.";
        return RedirectToAction(nameof(UploadImageList));
    }

    public IActionResult UploadImage(int id)
    {
        var product = _productRepository.GetById(id);
        if (product is null) return NotFound();
        return View(product);
    }


    [HttpPost]
    public IActionResult RemoveProductImage(int id)
    {
        var product = _productRepository.GetById(id);
        if (product is null) return NotFound();
        product.ImageUrl = null;
        _productRepository.Update(product);
        TempData["SuccessMessage"] = "Đã xóa ảnh sản phẩm.";
        return RedirectToAction(nameof(UploadImage), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage(int id, IFormFile imageFile)
    {
        var product = _productRepository.GetById(id);
        if (product is null) return NotFound();

        if (imageFile is null || imageFile.Length == 0)
        {
            ModelState.AddModelError(string.Empty, "Vui lòng chọn file ảnh.");
            return View(product);
        }

        var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        if (!allowed.Contains(extension))
        {
            ModelState.AddModelError(string.Empty, "Chỉ hỗ trợ JPG, PNG, WEBP.");
            return View(product);
        }

        var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "products");
        Directory.CreateDirectory(uploadsPath);

        var fileName = $"product-{id}-{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(uploadsPath, fileName);

        await using (var stream = System.IO.File.Create(fullPath))
        {
            await imageFile.CopyToAsync(stream);
        }

        product.ImageUrl = $"/uploads/products/{fileName}";
        _productRepository.Update(product);

        TempData["SuccessMessage"] = "Tải ảnh sản phẩm thành công.";
        return RedirectToAction(nameof(Display), new { id });
    }

    private void LoadCategories()
    {
        var categories = _categoryRepository.GetAllCategories();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
    }
}
