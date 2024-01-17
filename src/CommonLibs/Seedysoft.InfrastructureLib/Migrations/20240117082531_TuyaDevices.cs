using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seedysoft.InfrastructureLib.Migrations
{
    /// <inheritdoc />
    public partial class TuyaDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TuyaDevice",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<float>(type: "REAL", nullable: false),
                    LocalKey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuyaDevice", x => x.Id);
                });

            migrationBuilder.Sql("DROP VIEW IF EXISTS main.OutboxView");
            migrationBuilder.Sql("DROP VIEW IF EXISTS main.PvpcView");
            migrationBuilder.Sql("DROP VIEW IF EXISTS main.WebDataView");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TuyaDevice");
        }
    }
}
