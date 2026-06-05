using System.ComponentModel.DataAnnotations;

namespace webcofflatteshop.Models;

public class LoginViewModel
{
    [Required, StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

public class ForgotPasswordViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordViewModel
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class RegisterViewModel
{
    [Required, StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(300)]
    public string Address { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}

public class PurchaseHistoryViewModel
{
    public IEnumerable<Order> Orders { get; set; } = [];
    public bool IsAdmin { get; set; }
}

public class ProfilePageViewModel
{
    public ProfileFormViewModel Profile { get; set; } = new();

    public ChangePasswordViewModel Password { get; set; } = new();

    public EmailChangeViewModel EmailChange { get; set; } = new();

    public IEnumerable<string> Roles { get; set; } = [];

    public int OrderCount { get; set; }

    public decimal TotalSpent { get; set; }

    public DateTime? LastOrderAt { get; set; }

    public string? ProfileBackgroundImageUrl { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PendingEmail { get; set; }

    public int EmailCodeCooldownSeconds { get; set; }
}

public class ProfileFormViewModel
{
    [Required, StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(300)]
    public string Address { get; set; } = string.Empty;

    [Phone, StringLength(30)]
    public string? PhoneNumber { get; set; }
}

public class ChangePasswordViewModel
{
    [Required, DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(NewPassword))]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class EmailChangeViewModel
{
    [Required, EmailAddress]
    public string NewEmail { get; set; } = string.Empty;

    [StringLength(6, MinimumLength = 6)]
    public string VerificationCode { get; set; } = string.Empty;
}
