using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using webcofflatteshop.Models;

namespace webcofflatteshop.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.FullName)
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(user => user.Address)
                .HasMaxLength(300)
                .IsRequired();
            entity.Property(user => user.ProfileBackgroundImageUrl)
                .HasMaxLength(300);
            entity.Property(user => user.PendingEmail)
                .HasMaxLength(256);
            entity.Property(user => user.EmailVerificationCode)
                .HasMaxLength(10);
            entity.Property(user => user.EmailVerificationCodeExpiresAt)
                .HasColumnType("datetime2");
            entity.Property(user => user.EmailVerificationCodeSentAt)
                .HasColumnType("datetime2");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(category => category.Id);
            entity.Property(category => category.Name)
                .HasMaxLength(50)
                .IsRequired();
            entity.HasIndex(category => category.Name)
                .IsUnique();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(product => product.Id);
            entity.Property(product => product.Name)
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(product => product.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            entity.Property(product => product.Description)
                .HasMaxLength(500)
                .IsRequired();
            entity.Property(product => product.ImageUrl)
                .HasMaxLength(300);
            entity.Property(product => product.IsAvailable)
                .HasDefaultValue(true)
                .IsRequired();
            entity.Property(product => product.IsFeatured)
                .HasDefaultValue(false)
                .IsRequired();
            entity.Property(product => product.Stock)
                .HasDefaultValue(100)
                .IsRequired();
            entity.Property(product => product.CreatedAt)
                .HasColumnType("datetime2")
                .IsRequired();
            entity.Property(product => product.UpdatedAt)
                .HasColumnType("datetime2")
                .IsRequired();
            entity.HasOne(product => product.Category)
                .WithMany(category => category.Products)
                .HasForeignKey(product => product.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(order => order.Id);
            entity.Property(order => order.UserId)
                .HasMaxLength(450)
                .IsRequired();
            entity.Property(order => order.CustomerEmail)
                .HasMaxLength(256)
                .IsRequired();
            entity.Property(order => order.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            entity.Property(order => order.SubtotalAmount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0m)
                .IsRequired();
            entity.Property(order => order.ShippingFee)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0m)
                .IsRequired();
            entity.Property(order => order.FulfillmentMethod)
                .HasMaxLength(30)
                .HasDefaultValue("Pickup")
                .IsRequired();
            entity.Property(order => order.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Pending")
                .IsRequired();
            entity.Property(order => order.CreatedAt)
                .HasColumnType("datetime2")
                .IsRequired();
            entity.HasMany(order => order.Items)
                .WithOne(item => item.Order)
                .HasForeignKey(item => item.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItems");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.ProductName)
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(item => item.Sugar)
                .HasMaxLength(30)
                .IsRequired();
            entity.Property(item => item.Size)
                .HasMaxLength(30)
                .IsRequired();
            entity.Property(item => item.UnitPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            entity.Property(item => item.LineTotal)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            entity.HasOne(item => item.Product)
                .WithMany()
                .HasForeignKey(item => item.ProductId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.ToTable("ApiKeys");
            entity.HasKey(apiKey => apiKey.Id);
            entity.Property(apiKey => apiKey.UserId)
                .HasMaxLength(450)
                .IsRequired();
            entity.Property(apiKey => apiKey.Name)
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(apiKey => apiKey.KeyHash)
                .HasMaxLength(64)
                .IsRequired();
            entity.Property(apiKey => apiKey.KeyPrefix)
                .HasMaxLength(12)
                .IsRequired();
            entity.HasIndex(apiKey => apiKey.KeyHash)
                .IsUnique();
            entity.HasOne(apiKey => apiKey.User)
                .WithMany()
                .HasForeignKey(apiKey => apiKey.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        SeedMenu(modelBuilder);
    }

    private static void SeedMenu(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new { Id = 1, Name = "Coffee" },
            new { Id = 2, Name = "Matcha" },
            new { Id = 3, Name = "Chocolate" },
            new { Id = 4, Name = "Bakery" }
        );

        var seedDate = new DateTime(2026, 5, 29, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Product>().HasData(
            new { Id = 1, Name = "Espresso", Price = 25000m, Description = "Đậm đà vị cà phê Ý nguyên bản.", CategoryId = 1, ImageUrl = (string?)null, IsAvailable = true, IsFeatured = false, Stock = 100, CreatedAt = seedDate, UpdatedAt = seedDate },
            new { Id = 2, Name = "Americano", Price = 30000m, Description = "Espresso pha loãng, nhẹ nhàng và thơm.", CategoryId = 1, ImageUrl = (string?)null, IsAvailable = true, IsFeatured = false, Stock = 100, CreatedAt = seedDate, UpdatedAt = seedDate },
            new { Id = 3, Name = "Cappuccino", Price = 38000m, Description = "Bọt sữa mịn, cân bằng giữa sữa và cà phê.", CategoryId = 1, ImageUrl = (string?)null, IsAvailable = true, IsFeatured = false, Stock = 100, CreatedAt = seedDate, UpdatedAt = seedDate },
            new { Id = 4, Name = "Latte", Price = 42000m, Description = "Sữa béo mượt cùng espresso dịu êm.", CategoryId = 1, ImageUrl = (string?)null, IsAvailable = true, IsFeatured = false, Stock = 100, CreatedAt = seedDate, UpdatedAt = seedDate },
            new { Id = 5, Name = "Mocha", Price = 45000m, Description = "Hòa quyện cà phê và chocolate ngọt ngào.", CategoryId = 1, ImageUrl = (string?)null, IsAvailable = true, IsFeatured = false, Stock = 100, CreatedAt = seedDate, UpdatedAt = seedDate },
            new { Id = 6, Name = "Caramel Macchiato", Price = 49000m, Description = "Vị caramel thơm, hậu vị espresso mạnh.", CategoryId = 1, ImageUrl = (string?)null, IsAvailable = true, IsFeatured = false, Stock = 100, CreatedAt = seedDate, UpdatedAt = seedDate },
            new { Id = 7, Name = "Cold Brew", Price = 43000m, Description = "Ủ lạnh 18 tiếng, mượt và ít chua.", CategoryId = 1, ImageUrl = (string?)null, IsAvailable = true, IsFeatured = false, Stock = 100, CreatedAt = seedDate, UpdatedAt = seedDate },
            new { Id = 8, Name = "Vietnamese Iced Coffee", Price = 37000m, Description = "Cà phê sữa đá đậm vị Việt Nam.", CategoryId = 1, ImageUrl = (string?)null, IsAvailable = true, IsFeatured = false, Stock = 100, CreatedAt = seedDate, UpdatedAt = seedDate },
            new { Id = 9, Name = "Matcha Latte", Price = 46000m, Description = "Trà xanh Nhật kết hợp sữa thanh dịu.", CategoryId = 2, ImageUrl = (string?)null, IsAvailable = true, IsFeatured = false, Stock = 100, CreatedAt = seedDate, UpdatedAt = seedDate },
            new { Id = 10, Name = "Chocolate Frappe", Price = 52000m, Description = "Đá xay chocolate mát lạnh cho ngày hè.", CategoryId = 3, ImageUrl = (string?)null, IsAvailable = true, IsFeatured = false, Stock = 100, CreatedAt = seedDate, UpdatedAt = seedDate },
            new { Id = 11, Name = "Croissant Butter", Price = 29000m, Description = "Bánh sừng bò bơ giòn tan mỗi sáng.", CategoryId = 4, ImageUrl = (string?)null, IsAvailable = true, IsFeatured = false, Stock = 100, CreatedAt = seedDate, UpdatedAt = seedDate },
            new { Id = 12, Name = "Tiramisu", Price = 41000m, Description = "Bánh tiramisu mềm mịn thơm cà phê.", CategoryId = 4, ImageUrl = (string?)null, IsAvailable = true, IsFeatured = false, Stock = 100, CreatedAt = seedDate, UpdatedAt = seedDate },
            new { Id = 13, Name = "Blueberry Cheesecake", Price = 48000m, Description = "Cheesecake béo nhẹ phủ mứt việt quất.", CategoryId = 4, ImageUrl = (string?)null, IsAvailable = true, IsFeatured = false, Stock = 100, CreatedAt = seedDate, UpdatedAt = seedDate }
        );
    }
}
