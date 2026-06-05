using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace webcofflatteshop.Models;

public class ApplicationUser : IdentityUser
{
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(300)]
    public string Address { get; set; } = string.Empty;

    [StringLength(300)]
    public string? ProfileBackgroundImageUrl { get; set; }

    [StringLength(256)]
    public string? PendingEmail { get; set; }

    [StringLength(10)]
    public string? EmailVerificationCode { get; set; }

    public DateTime? EmailVerificationCodeExpiresAt { get; set; }

    public DateTime? EmailVerificationCodeSentAt { get; set; }
}
