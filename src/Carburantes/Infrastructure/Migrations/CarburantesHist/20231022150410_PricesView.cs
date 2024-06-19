using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seedysoft.Carburantes.Infrastructure.Migrations.CarburantesHist
{
    /// <inheritdoc />
    public partial class PricesView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql($"{Views.ProductPriceHistView.GetDropSql().Replace("'", "''")}");

            _ = migrationBuilder.Sql($"{Views.ProductPriceHistView.GetCreateSql(Views.ProductPriceHistView.VersionNumbers.Version20221230).Replace("'", "''")}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql($"{Views.ProductPriceHistView.GetDropSql().Replace("'", "''")}");

            _ = migrationBuilder.Sql($"{Views.ProductPriceHistView.GetCreateSqlPrevious(Views.ProductPriceHistView.VersionNumbers.Version20221230).Replace("'", "''")}");
        }
    }
}
