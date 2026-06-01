using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webcofflatteshop.Migrations
{
    /// <inheritdoc />
    [Migration("20260601000000_NormalizeProductPricesToVnd")]
    public partial class NormalizeProductPricesToVnd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            UpdateSeedPrices(migrationBuilder,
                25000m, 30000m, 38000m, 42000m, 45000m, 49000m, 43000m,
                37000m, 46000m, 52000m, 29000m, 41000m, 48000m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            UpdateSeedPrices(migrationBuilder,
                2.50m, 3.00m, 3.80m, 4.20m, 4.50m, 4.90m, 4.30m,
                3.70m, 4.60m, 5.20m, 2.90m, 4.10m, 4.80m);
        }

        private static void UpdateSeedPrices(MigrationBuilder migrationBuilder, params decimal[] prices)
        {
            for (var index = 0; index < prices.Length; index++)
            {
                migrationBuilder.UpdateData(
                    table: "Products",
                    keyColumn: "Id",
                    keyValue: index + 1,
                    column: "Price",
                    value: prices[index]);
            }
        }
    }
}
