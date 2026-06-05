using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using webcofflatteshop.Data;
using webcofflatteshop.Models;
using webcofflatteshop.Services;

namespace webcofflatteshop.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IVerificationEmailSender _emailSender;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        IVerificationEmailSender emailSender,
        IWebHostEnvironment webHostEnvironment)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _context = context;
        _emailSender = emailSender;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
        if (result.Succeeded)
        {
            return LocalRedirect(SafeReturnUrl(model.ReturnUrl));
        }

        ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
        return View(model);
    }

    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            TempData["ForgotPasswordMessage"] = "Nếu email tồn tại trong hệ thống, liên kết đặt lại mật khẩu sẽ được gửi đến hộp thư của bạn.";
            return RedirectToAction(nameof(ForgotPassword));
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = Url.Action(nameof(ResetPassword), "Account", new { userId = user.Id, token }, Request.Scheme);
        if (string.IsNullOrWhiteSpace(resetLink))
        {
            ModelState.AddModelError(string.Empty, "Không tạo được liên kết đặt lại mật khẩu.");
            return View(model);
        }

        try
        {
            await _emailSender.SendPasswordResetLinkAsync(model.Email, resetLink);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }

        TempData["ForgotPasswordMessage"] = "Nếu email tồn tại trong hệ thống, liên kết đặt lại mật khẩu sẽ được gửi đến hộp thư của bạn.";
        return RedirectToAction(nameof(ForgotPassword));
    }

    public async Task<IActionResult> ResetPassword(string? userId = null, string? token = null)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
        {
            return RedirectToAction(nameof(Login));
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return RedirectToAction(nameof(Login));
        }

        return View(new ResetPasswordViewModel { UserId = userId, Token = token });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user is null)
        {
            return RedirectToAction(nameof(Login));
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (!result.Succeeded)
        {
            AddIdentityErrors(result);
            return View(model);
        }

        TempData["LoginMessage"] = "Đã cập nhật mật khẩu. Bạn có thể đăng nhập bằng mật khẩu mới.";
        return RedirectToAction(nameof(Login));
    }

    public IActionResult Register(string? returnUrl = null)
    {
        return View(new RegisterViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email,
            EmailConfirmed = false,
            FullName = model.FullName,
            Address = model.Address
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Customer");
            await _signInManager.SignInAsync(user, false);
            TempData["RegisterNotice"] = "Bạn hãy nhớ liên kết với mã xác thực trong trang cá nhân nhé.";
            return LocalRedirect(SafeReturnUrl(model.ReturnUrl));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Product");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> PurchaseHistory()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        var orders = await _context.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .Where(order => order.UserId == user.Id)
            .OrderByDescending(order => order.CreatedAt)
            .ToListAsync();

        return View(new PurchaseHistoryViewModel { Orders = orders });
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        return View(await BuildProfilePageAsync(user));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile([Bind(Prefix = "Profile")] ProfileFormViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        if (!ModelState.IsValid)
        {
            return View("Profile", await BuildProfilePageAsync(user, model, new ChangePasswordViewModel()));
        }

        if (!string.Equals(user.UserName, model.UserName, StringComparison.OrdinalIgnoreCase))
        {
            var userNameResult = await _userManager.SetUserNameAsync(user, model.UserName);
            if (!userNameResult.Succeeded)
            {
                AddIdentityErrors(userNameResult);
                return View("Profile", await BuildProfilePageAsync(user, model, new ChangePasswordViewModel()));
            }
        }

        user.FullName = model.FullName;
        user.Address = model.Address;
        user.PhoneNumber = model.PhoneNumber;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            AddIdentityErrors(updateResult);
            return View("Profile", await BuildProfilePageAsync(user, model, new ChangePasswordViewModel()));
        }

        await _signInManager.RefreshSignInAsync(user);
        TempData["ProfileSuccess"] = "Đã cập nhật thông tin cá nhân.";
        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword([Bind(Prefix = "Password")] ChangePasswordViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        if (!ModelState.IsValid)
        {
            return View("Profile", await BuildProfilePageAsync(user, null, model));
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            AddIdentityErrors(result);
            return View("Profile", await BuildProfilePageAsync(user, null, model));
        }

        await _signInManager.RefreshSignInAsync(user);
        TempData["PasswordSuccess"] = "Đã đổi mật khẩu thành công.";
        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendEmailVerification([Bind(Prefix = "EmailChange")] EmailChangeViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        if (string.IsNullOrWhiteSpace(model.NewEmail))
        {
            TempData["EmailError"] = "Vui lòng nhập Gmail cần xác thực.";
            return RedirectToAction(nameof(Profile));
        }

        if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(model.NewEmail))
        {
            TempData["EmailError"] = "Định dạng Gmail không hợp lệ.";
            return RedirectToAction(nameof(Profile));
        }

        var cooldown = GetEmailCodeCooldownSeconds(user);
        if (cooldown > 0)
        {
            TempData["EmailError"] = $"Vui lòng chờ {cooldown} giây trước khi gửi mã mới.";
            return RedirectToAction(nameof(Profile));
        }

        var existingUser = await _userManager.FindByEmailAsync(model.NewEmail);
        if (existingUser is not null && existingUser.Id != user.Id)
        {
            TempData["EmailError"] = "Gmail này đã được tài khoản khác sử dụng.";
            return RedirectToAction(nameof(Profile));
        }

        var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        try
        {
            await _emailSender.SendEmailVerificationCodeAsync(model.NewEmail, code);
        }
        catch (Exception ex)
        {
            TempData["EmailError"] = ex.Message;
            return RedirectToAction(nameof(Profile));
        }

        user.PendingEmail = model.NewEmail;
        user.EmailVerificationCode = code;
        user.EmailVerificationCodeExpiresAt = DateTime.UtcNow.AddMinutes(10);
        user.EmailVerificationCodeSentAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        TempData["EmailSuccess"] = $"Đã gửi mã xác thực đến {model.NewEmail}. Mã có hiệu lực trong 10 phút.";
        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyEmailCode([Bind(Prefix = "EmailChange")] EmailChangeViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        if (string.IsNullOrWhiteSpace(user.PendingEmail) || string.IsNullOrWhiteSpace(user.EmailVerificationCode))
        {
            TempData["EmailError"] = "Chưa có Gmail nào đang chờ xác thực.";
            return RedirectToAction(nameof(Profile));
        }

        if (user.EmailVerificationCodeExpiresAt is null || user.EmailVerificationCodeExpiresAt < DateTime.UtcNow)
        {
            TempData["EmailError"] = "Mã xác thực đã hết hạn. Vui lòng gửi mã mới.";
            return RedirectToAction(nameof(Profile));
        }

        if (!string.Equals(user.EmailVerificationCode, model.VerificationCode, StringComparison.Ordinal))
        {
            TempData["EmailError"] = "Mã xác thực không đúng.";
            return RedirectToAction(nameof(Profile));
        }

        var pendingEmail = user.PendingEmail!;
        var setEmailResult = await _userManager.SetEmailAsync(user, pendingEmail);
        if (!setEmailResult.Succeeded)
        {
            AddIdentityErrors(setEmailResult);
            return View("Profile", await BuildProfilePageAsync(user, null, new ChangePasswordViewModel(), model));
        }

        user.EmailConfirmed = true;
        user.PendingEmail = null;
        user.EmailVerificationCode = null;
        user.EmailVerificationCodeExpiresAt = null;
        user.EmailVerificationCodeSentAt = null;
        await _userManager.UpdateAsync(user);
        await _signInManager.RefreshSignInAsync(user);

        TempData["EmailSuccess"] = "Gmail đã được xác thực và cập nhật cho tài khoản.";
        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadProfileBackground(IFormFile backgroundFile)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        if (backgroundFile is null || backgroundFile.Length == 0)
        {
            TempData["ProfileError"] = "Vui lòng chọn ảnh đại diện.";
            return RedirectToAction(nameof(Profile));
        }

        var extension = Path.GetExtension(backgroundFile.FileName).ToLowerInvariant();
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        if (!allowed.Contains(extension))
        {
            TempData["ProfileError"] = "Ảnh đại diện chỉ hỗ trợ JPG, PNG, WEBP.";
            return RedirectToAction(nameof(Profile));
        }

        var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profiles");
        Directory.CreateDirectory(uploadsPath);

        var fileName = $"profile-bg-{user.Id}-{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(uploadsPath, fileName);

        await using (var stream = System.IO.File.Create(fullPath))
        {
            await backgroundFile.CopyToAsync(stream);
        }

        user.ProfileBackgroundImageUrl = $"/uploads/profiles/{fileName}";
        await _userManager.UpdateAsync(user);

        TempData["ProfileSuccess"] = "Đã cập nhật ảnh đại diện tài khoản.";
        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveProfileBackground()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        user.ProfileBackgroundImageUrl = null;
        await _userManager.UpdateAsync(user);

        TempData["ProfileSuccess"] = "Đã xóa ảnh đại diện tài khoản.";
        return RedirectToAction(nameof(Profile));
    }

    private string SafeReturnUrl(string? returnUrl)
    {
        return Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Action("Index", "Product")!;
    }

    private async Task<ProfilePageViewModel> BuildProfilePageAsync(
        ApplicationUser user,
        ProfileFormViewModel? profile = null,
        ChangePasswordViewModel? password = null,
        EmailChangeViewModel? emailChange = null)
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .Where(order => order.UserId == user.Id)
            .ToListAsync();

        return new ProfilePageViewModel
        {
            Profile = profile ?? new ProfileFormViewModel
            {
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
            },
            Password = password ?? new ChangePasswordViewModel(),
            EmailChange = emailChange ?? new EmailChangeViewModel
            {
                NewEmail = user.PendingEmail ?? user.Email ?? string.Empty
            },
            Roles = await _userManager.GetRolesAsync(user),
            OrderCount = orders.Count,
            TotalSpent = orders.Sum(order => order.TotalAmount),
            LastOrderAt = orders.OrderByDescending(order => order.CreatedAt).FirstOrDefault()?.CreatedAt,
            ProfileBackgroundImageUrl = user.ProfileBackgroundImageUrl,
            EmailConfirmed = user.EmailConfirmed,
            PendingEmail = user.PendingEmail,
            EmailCodeCooldownSeconds = GetEmailCodeCooldownSeconds(user)
        };
    }

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    private static int GetEmailCodeCooldownSeconds(ApplicationUser user)
    {
        if (user.EmailVerificationCodeSentAt is null) return 0;

        var elapsed = DateTime.UtcNow - user.EmailVerificationCodeSentAt.Value;
        return Math.Max(0, 30 - (int)Math.Floor(elapsed.TotalSeconds));
    }
}
