using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seedysoft.InfrastructureLib.Migrations
{
    /// <inheritdoc />
    public partial class WebDataUseHttpClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UseHttpClient",
                table: "WebData",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            //_ = migrationBuilder.Sql($"{Views.WebDataView.GetDropSql()}");
            //_ = migrationBuilder.Sql($"{Views.WebDataView.GetCreateSql(Views.WebDataView.VersionNumbers.Version20231112)}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseHttpClient",
                table: "WebData");

            //_ = migrationBuilder.Sql($"{Views.WebDataView.GetDropSql()}");
            //_ = migrationBuilder.Sql($"{Views.WebDataView.GetCreateSqlPrevious(Views.WebDataView.VersionNumbers.Version20231112)}");
        }
    }
}
