using System.ComponentModel.DataAnnotations;

namespace webcofflatteshop.Models;

public class ApiKey
{
    public int Id { get; set; }

    [Required, StringLength(450)]
    public string UserId { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(64)]
    public string KeyHash { get; set; } = string.Empty;

    [Required, StringLength(12)]
    public string KeyPrefix { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastUsedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public ApplicationUser? User { get; set; }
}
