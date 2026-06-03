using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webcofflatteshop.Data;
using webcofflatteshop.Models;
using webcofflatteshop.Repository;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found. Copy your SQL Server connection string into appsettings.json before adding migrations.");

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/Product/Checkout"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});
builder.Services.AddScoped<IProductRepository, EfProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EfCategoryRepository>();
builder.Services.AddSingleton<IBannerRepository, FileBannerRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await SeedIdentityAsync(app.Services);

app.Run();

static async Task SeedIdentityAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    await context.Database.MigrateAsync();
    await EnsureIdentityProfileColumnsAsync(context);

    foreach (var roleName in new[] { "Admin", "Customer" })
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    const string adminEmail = "admin@coffeelatte.local";
    const string adminPassword = "Admin@12345";
    var admin = await userManager.FindByEmailAsync(adminEmail)
        ?? await userManager.FindByNameAsync(adminEmail);
    if (admin is null)
    {
        admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FullName = "Admin",
            Address = "Coffee Latte Shop"
        };
        var createResult = await userManager.CreateAsync(admin, adminPassword);
        if (!createResult.Succeeded)
        {
            var errors = string.Join("; ", createResult.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Không thể tạo tài khoản admin mặc định: {errors}");
        }
    }
    else if (string.IsNullOrWhiteSpace(admin.FullName) || string.IsNullOrWhiteSpace(admin.Address))
    {
        admin.UserName = string.IsNullOrWhiteSpace(admin.UserName) ? adminEmail : admin.UserName;
        admin.Email = string.IsNullOrWhiteSpace(admin.Email) ? adminEmail : admin.Email;
        admin.FullName = string.IsNullOrWhiteSpace(admin.FullName) ? "Admin" : admin.FullName;
        admin.Address = string.IsNullOrWhiteSpace(admin.Address) ? "Coffee Latte Shop" : admin.Address;
        var updateResult = await userManager.UpdateAsync(admin);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join("; ", updateResult.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Không thể cập nhật tài khoản admin mặc định: {errors}");
        }
    }

    if (!await userManager.IsInRoleAsync(admin, "Admin"))
    {
        var roleResult = await userManager.AddToRoleAsync(admin, "Admin");
        if (!roleResult.Succeeded)
        {
            var errors = string.Join("; ", roleResult.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Không thể gán quyền Admin cho tài khoản mặc định: {errors}");
        }
    }
}

static async Task EnsureIdentityProfileColumnsAsync(ApplicationDbContext context)
{
    await context.Database.ExecuteSqlRawAsync("""
        IF COL_LENGTH('AspNetUsers', 'FullName') IS NULL
        BEGIN
            ALTER TABLE AspNetUsers ADD FullName nvarchar(100) NOT NULL CONSTRAINT DF_AspNetUsers_FullName DEFAULT N''
        END
        """);

    await context.Database.ExecuteSqlRawAsync("""
        IF COL_LENGTH('AspNetUsers', 'Address') IS NULL
        BEGIN
            ALTER TABLE AspNetUsers ADD Address nvarchar(300) NOT NULL CONSTRAINT DF_AspNetUsers_Address DEFAULT N''
        END
        """);
}
