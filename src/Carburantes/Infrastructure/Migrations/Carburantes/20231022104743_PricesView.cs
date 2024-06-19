using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seedysoft.Carburantes.Infrastructure.Migrations.Carburantes
{
    /// <inheritdoc />
    public partial class PricesView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql($"{Views.ProductPriceView.GetDropSql().Replace("'", "''")}");

            _ = migrationBuilder.Sql($"{Views.ProductPriceView.GetCreateSql(Views.ProductPriceView.VersionNumbers.Version20221230).Replace("'", "''")}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql($"{Views.ProductPriceView.GetDropSql().Replace("'", "''")}");

            _ = migrationBuilder.Sql($"{Views.ProductPriceView.GetCreateSqlPrevious(Views.ProductPriceView.VersionNumbers.Version20221230).Replace("'", "''")}");
        }
    }
}
