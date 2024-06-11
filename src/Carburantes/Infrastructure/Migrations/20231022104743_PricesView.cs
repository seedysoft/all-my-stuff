using Microsoft.EntityFrameworkCore.Migrations;
using Seedysoft.Carburantes.Infrastructure.Migrations.Views;

#nullable disable

namespace Seedysoft.Carburantes.Infrastructure.Migrations.Carburantes
{
    /// <inheritdoc />
    public partial class PricesView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql($"{ProductPriceView.GetDropSql().Replace("'", "''")}");

            _ = migrationBuilder.Sql($"{ProductPriceView.GetCreateSql(ProductPriceView.VersionNumbers.Version20221230).Replace("'", "''")}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql($"{ProductPriceView.GetDropSql().Replace("'", "''")}");

            _ = migrationBuilder.Sql($"{ProductPriceView.GetCreateSqlPrevious(ProductPriceView.VersionNumbers.Version20221230).Replace("'", "''")}");
        }
    }
}
