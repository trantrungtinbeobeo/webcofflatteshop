using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace webcofflatteshop.Migrations
{
    /// <inheritdoc />
    public partial class NewDBLocal : Migration
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
                    ImageUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
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
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 1, 1, "Đậm đà vị cà phê Ý nguyên bản.", null, "Espresso", 2.50m },
                    { 2, 1, "Espresso pha loãng, nhẹ nhàng và thơm.", null, "Americano", 3.00m },
                    { 3, 1, "Bọt sữa mịn, cân bằng giữa sữa và cà phê.", null, "Cappuccino", 3.80m },
                    { 4, 1, "Sữa béo mượt cùng espresso dịu êm.", null, "Latte", 4.20m },
                    { 5, 1, "Hòa quyện cà phê và chocolate ngọt ngào.", null, "Mocha", 4.50m },
                    { 6, 1, "Vị caramel thơm, hậu vị espresso mạnh.", null, "Caramel Macchiato", 4.90m },
                    { 7, 1, "Ủ lạnh 18 tiếng, mượt và ít chua.", null, "Cold Brew", 4.30m },
                    { 8, 1, "Cà phê sữa đá đậm vị Việt Nam.", null, "Vietnamese Iced Coffee", 3.70m },
                    { 9, 2, "Trà xanh Nhật kết hợp sữa thanh dịu.", null, "Matcha Latte", 4.60m },
                    { 10, 3, "Đá xay chocolate mát lạnh cho ngày hè.", null, "Chocolate Frappe", 5.20m },
                    { 11, 4, "Bánh sừng bò bơ giòn tan mỗi sáng.", null, "Croissant Butter", 2.90m },
                    { 12, 4, "Bánh tiramisu mềm mịn thơm cà phê.", null, "Tiramisu", 4.10m },
                    { 13, 4, "Cheesecake béo nhẹ phủ mứt việt quất.", null, "Blueberry Cheesecake", 4.80m }
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
