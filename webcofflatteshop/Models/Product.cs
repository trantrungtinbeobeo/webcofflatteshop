using System.ComponentModel.DataAnnotations;

namespace webcofflatteshop.Models
{
// Product.cs
public class Product
{
    public int Id { get; set; }
    [Required, StringLength(100)]
    public string Name { get; set; }
    [Range(0.01, 10000.00)]
    public decimal Price { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }
}
// Category.cs
public class Category
{
    public int Id { get; set; }
    [Required, StringLength(50)]
    public string Name { get; set; }
}
}