using Microsoft.EntityFrameworkCore.Migrations;
using Seedysoft.FuelPrices.Lib.Infrastructure.Migrations.CarburantesHist.Views;

#nullable disable

namespace Seedysoft.Infrastructure.Migrations.CarburantesHist
{
    /// <inheritdoc />
    public partial class PricesView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql($"{ProductPriceHistView.GetDropSql().Replace("'", "''")}");

            _ = migrationBuilder.Sql($"{ProductPriceHistView.GetCreateSql(ProductPriceHistView.VersionNumbers.Version20221230).Replace("'", "''")}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql($"{ProductPriceHistView.GetDropSql().Replace("'", "''")}");

            _ = migrationBuilder.Sql($"{ProductPriceHistView.GetCreateSqlPrevious(ProductPriceHistView.VersionNumbers.Version20221230).Replace("'", "''")}");
        }
    }
}
