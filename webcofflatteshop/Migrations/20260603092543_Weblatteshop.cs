using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace webcofflatteshop.Migrations
{
    /// <inheritdoc />
    public partial class Weblatteshop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Stock = table.Column<int>(type: "int", nullable: false, defaultValue: 100),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Coffee" },
                    { 2, "Matcha" },
                    { 3, "Chocolate" },
                    { 4, "Bakery" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "ImageUrl", "IsAvailable", "Name", "Price", "Stock", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Đậm đà vị cà phê Ý nguyên bản.", null, true, "Espresso", 25000m, 100, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 1, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Espresso pha loãng, nhẹ nhàng và thơm.", null, true, "Americano", 30000m, 100, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 1, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Bọt sữa mịn, cân bằng giữa sữa và cà phê.", null, true, "Cappuccino", 38000m, 100, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 1, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Sữa béo mượt cùng espresso dịu êm.", null, true, "Latte", 42000m, 100, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, 1, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Hòa quyện cà phê và chocolate ngọt ngào.", null, true, "Mocha", 45000m, 100, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, 1, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Vị caramel thơm, hậu vị espresso mạnh.", null, true, "Caramel Macchiato", 49000m, 100, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, 1, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Ủ lạnh 18 tiếng, mượt và ít chua.", null, true, "Cold Brew", 43000m, 100, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, 1, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Cà phê sữa đá đậm vị Việt Nam.", null, true, "Vietnamese Iced Coffee", 37000m, 100, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, 2, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Trà xanh Nhật kết hợp sữa thanh dịu.", null, true, "Matcha Latte", 46000m, 100, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, 3, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Đá xay chocolate mát lạnh cho ngày hè.", null, true, "Chocolate Frappe", 52000m, 100, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, 4, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Bánh sừng bò bơ giòn tan mỗi sáng.", null, true, "Croissant Butter", 29000m, 100, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, 4, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Bánh tiramisu mềm mịn thơm cà phê.", null, true, "Tiramisu", 41000m, 100, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, 4, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Cheesecake béo nhẹ phủ mứt việt quất.", null, true, "Blueberry Cheesecake", 48000m, 100, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
