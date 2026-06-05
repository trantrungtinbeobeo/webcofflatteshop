using System.ComponentModel.DataAnnotations;

namespace webcofflatteshop.Models;

public class Order
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string CustomerEmail { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public decimal SubtotalAmount { get; set; }

    public decimal ShippingFee { get; set; }

    [StringLength(30)]
    public string FulfillmentMethod { get; set; } = "Pickup";

    [StringLength(30)]
    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<OrderItem> Items { get; set; } = [];
}

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int? ProductId { get; set; }

    [Required, StringLength(100)]
    public string ProductName { get; set; } = string.Empty;

    [Range(1, 1000)]
    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }

    [StringLength(30)]
    public string Sugar { get; set; } = string.Empty;

    [StringLength(30)]
    public string Size { get; set; } = string.Empty;

    public Order? Order { get; set; }

    public Product? Product { get; set; }
}
