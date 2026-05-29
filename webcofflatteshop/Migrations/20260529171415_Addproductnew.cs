using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webcofflatteshop.Migrations
{
    /// <inheritdoc />
    public partial class Addproductnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "IsAvailable", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "IsAvailable", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "IsAvailable", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "IsAvailable", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "IsAvailable", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "IsAvailable", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "IsAvailable", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "IsAvailable", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "IsAvailable", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "IsAvailable", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "IsAvailable", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAt", "IsAvailable", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedAt", "IsAvailable", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Products");
        }
    }
}
