using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webcofflatteshop.Authentication;
using webcofflatteshop.Data;
using webcofflatteshop.Hubs;
using webcofflatteshop.Models;

namespace webcofflatteshop.Controllers.Api;

[ApiController]
[Route("api/products")]
[Authorize(AuthenticationSchemes = ApiKeyAuthenticationDefaults.AuthenticationScheme)]
public class ProductApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<ProductRealtimeHub> _productHub;

    public ProductApiController(ApplicationDbContext context, IHubContext<ProductRealtimeHub> productHub)
    {
        _context = context;
        _productHub = productHub;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductApiResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductApiResponse>>> GetProducts()
    {
        var products = await ProductQuery()
            .OrderBy(product => product.Name)
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductApiResponse>> GetProduct(int id)
    {
        var product = await ProductQuery()
            .FirstOrDefaultAsync(product => product.Id == id);

        return product is null
            ? NotFound(new { message = $"Không tìm thấy sản phẩm có mã {id}." })
            : Ok(product);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductApiResponse>> CreateProduct(ProductApiRequest request)
    {
        if (!await _context.Categories.AnyAsync(category => category.Id == request.CategoryId))
        {
            ModelState.AddModelError(nameof(request.CategoryId), "Danh mục không tồn tại.");
            return ValidationProblem(ModelState);
        }

        var product = new Product
        {
            Name = request.Name.Trim(),
            Price = request.Price,
            Description = request.Description.Trim(),
            ImageUrl = NormalizeImageUrl(request.ImageUrl),
            Stock = request.Stock,
            IsAvailable = request.IsAvailable,
            IsFeatured = request.IsFeatured,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var response = await ProductQuery().FirstAsync(item => item.Id == product.Id);
        await NotifyProductChangedAsync("created", response);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductApiResponse>> UpdateProduct(int id, ProductApiRequest request)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound(new { message = $"Không tìm thấy sản phẩm có mã {id}." });

        if (!await _context.Categories.AnyAsync(category => category.Id == request.CategoryId))
        {
            ModelState.AddModelError(nameof(request.CategoryId), "Danh mục không tồn tại.");
            return ValidationProblem(ModelState);
        }

        product.Name = request.Name.Trim();
        product.Price = request.Price;
        product.Description = request.Description.Trim();
        product.ImageUrl = NormalizeImageUrl(request.ImageUrl);
        product.Stock = request.Stock;
        product.IsAvailable = request.IsAvailable;
        product.IsFeatured = request.IsFeatured;
        product.CategoryId = request.CategoryId;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var response = await ProductQuery().FirstAsync(item => item.Id == product.Id);
        await NotifyProductChangedAsync("updated", response);
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound(new { message = $"Không tìm thấy sản phẩm có mã {id}." });

        var deletedProduct = new ProductRealtimeResponse
        {
            Id = product.Id,
            Name = product.Name
        };

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        await _productHub.Clients.All.SendAsync("ProductDeleted", deletedProduct);
        return NoContent();
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<CategoryApiResponse>>> GetCategories()
    {
        return Ok(await _context.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new CategoryApiResponse { Id = category.Id, Name = category.Name })
            .ToListAsync());
    }

    private static string? NormalizeImageUrl(string? imageUrl) =>
        string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim();

    private IQueryable<ProductApiResponse> ProductQuery()
    {
        return _context.Products
            .AsNoTracking()
            .Select(product => new ProductApiResponse
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Stock = product.Stock,
                IsAvailable = product.IsAvailable,
                IsFeatured = product.IsFeatured,
                CategoryId = product.CategoryId,
                CategoryName = product.Category == null ? null : product.Category.Name,
                UpdatedAt = product.UpdatedAt
            });
    }

    private Task NotifyProductChangedAsync(string action, ProductApiResponse product)
    {
        var payload = new ProductRealtimeResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            Stock = product.Stock,
            IsAvailable = product.IsAvailable,
            IsFeatured = product.IsFeatured,
            CategoryId = product.CategoryId,
            CategoryName = product.CategoryName,
            UpdatedAt = product.UpdatedAt
        };

        return action == "created"
            ? _productHub.Clients.All.SendAsync("ProductCreated", payload)
            : _productHub.Clients.All.SendAsync("ProductUpdated", payload);
    }
}

public class ProductApiRequest
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.StringLength(100)]
    [System.ComponentModel.DefaultValue("Latte API Demo")]
    public string Name { get; set; } = "Latte API Demo";

    [System.ComponentModel.DataAnnotations.Range(1000, 10000000)]
    [System.ComponentModel.DefaultValue(42000)]
    public decimal Price { get; set; } = 42000m;

    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.StringLength(500)]
    [System.ComponentModel.DefaultValue("San pham demo duoc tao tu Swagger RESTful API.")]
    public string Description { get; set; } = "San pham demo duoc tao tu Swagger RESTful API.";

    [System.ComponentModel.DataAnnotations.StringLength(300)]
    [System.ComponentModel.DefaultValue("/images/default-coffee.svg")]
    public string? ImageUrl { get; set; } = "/images/default-coffee.svg";

    [System.ComponentModel.DataAnnotations.Range(0, 100000)]
    [System.ComponentModel.DefaultValue(25)]
    public int Stock { get; set; } = 25;

    [System.ComponentModel.DefaultValue(true)]
    public bool IsAvailable { get; set; } = true;

    [System.ComponentModel.DefaultValue(false)]
    public bool IsFeatured { get; set; }

    [System.ComponentModel.DefaultValue(1)]
    public int CategoryId { get; set; } = 1;
}

public class CategoryApiResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class ProductApiResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int Stock { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsFeatured { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ProductRealtimeResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int Stock { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsFeatured { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public DateTime UpdatedAt { get; set; }
}
