using Microsoft.EntityFrameworkCore;
using webcofflatteshop.Models;

namespace webcofflatteshop.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
            entity.HasOne(product => product.Category)
                .WithMany(category => category.Products)
                .HasForeignKey(product => product.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
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

        modelBuilder.Entity<Product>().HasData(
            new { Id = 1, Name = "Espresso", Price = 2.50m, Description = "Đậm đà vị cà phê Ý nguyên bản.", CategoryId = 1, ImageUrl = (string?)null },
            new { Id = 2, Name = "Americano", Price = 3.00m, Description = "Espresso pha loãng, nhẹ nhàng và thơm.", CategoryId = 1, ImageUrl = (string?)null },
            new { Id = 3, Name = "Cappuccino", Price = 3.80m, Description = "Bọt sữa mịn, cân bằng giữa sữa và cà phê.", CategoryId = 1, ImageUrl = (string?)null },
            new { Id = 4, Name = "Latte", Price = 4.20m, Description = "Sữa béo mượt cùng espresso dịu êm.", CategoryId = 1, ImageUrl = (string?)null },
            new { Id = 5, Name = "Mocha", Price = 4.50m, Description = "Hòa quyện cà phê và chocolate ngọt ngào.", CategoryId = 1, ImageUrl = (string?)null },
            new { Id = 6, Name = "Caramel Macchiato", Price = 4.90m, Description = "Vị caramel thơm, hậu vị espresso mạnh.", CategoryId = 1, ImageUrl = (string?)null },
            new { Id = 7, Name = "Cold Brew", Price = 4.30m, Description = "Ủ lạnh 18 tiếng, mượt và ít chua.", CategoryId = 1, ImageUrl = (string?)null },
            new { Id = 8, Name = "Vietnamese Iced Coffee", Price = 3.70m, Description = "Cà phê sữa đá đậm vị Việt Nam.", CategoryId = 1, ImageUrl = (string?)null },
            new { Id = 9, Name = "Matcha Latte", Price = 4.60m, Description = "Trà xanh Nhật kết hợp sữa thanh dịu.", CategoryId = 2, ImageUrl = (string?)null },
            new { Id = 10, Name = "Chocolate Frappe", Price = 5.20m, Description = "Đá xay chocolate mát lạnh cho ngày hè.", CategoryId = 3, ImageUrl = (string?)null },
            new { Id = 11, Name = "Croissant Butter", Price = 2.90m, Description = "Bánh sừng bò bơ giòn tan mỗi sáng.", CategoryId = 4, ImageUrl = (string?)null },
            new { Id = 12, Name = "Tiramisu", Price = 4.10m, Description = "Bánh tiramisu mềm mịn thơm cà phê.", CategoryId = 4, ImageUrl = (string?)null },
            new { Id = 13, Name = "Blueberry Cheesecake", Price = 4.80m, Description = "Cheesecake béo nhẹ phủ mứt việt quất.", CategoryId = 4, ImageUrl = (string?)null }
        );
    }
}
