using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seedysoft.Libs.FuelPrices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Creating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComunidadAutonoma",
                columns: table => new
                {
                    AtDate = table.Column<int>(type: "INTEGER", nullable: false),
                    IdComunidadAutonoma = table.Column<int>(type: "INTEGER", nullable: false),
                    NombreComunidadAutonoma = table.Column<string>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComunidadAutonoma", x => new { x.IdComunidadAutonoma, x.AtDate });
                });

            migrationBuilder.CreateTable(
                name: "EstacionProductoPrecio",
                columns: table => new
                {
                    AtDate = table.Column<int>(type: "INTEGER", nullable: false),
                    IdEstacion = table.Column<int>(type: "INTEGER", nullable: false),
                    IdProducto = table.Column<int>(type: "INTEGER", nullable: false),
                    CentimosDeEuro = table.Column<int>(type: "INTEGER", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstacionProductoPrecio", x => new { x.IdEstacion, x.IdProducto, x.AtDate });
                });

            migrationBuilder.CreateTable(
                name: "EstacionServicio",
                columns: table => new
                {
                    AtDate = table.Column<int>(type: "INTEGER", nullable: false),
                    IdEstacion = table.Column<int>(type: "INTEGER", nullable: false),
                    IdMunicipio = table.Column<int>(type: "INTEGER", nullable: false),
                    CodigoPostal = table.Column<string>(type: "TEXT", nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", nullable: false),
                    Horario = table.Column<string>(type: "TEXT", nullable: false),
                    Latitud = table.Column<string>(type: "TEXT", nullable: false),
                    Localidad = table.Column<string>(type: "TEXT", nullable: false),
                    LongitudWgs84 = table.Column<string>(type: "TEXT", nullable: false),
                    Margen = table.Column<string>(type: "TEXT", nullable: false),
                    Rotulo = table.Column<string>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstacionServicio", x => new { x.IdEstacion, x.AtDate });
                });

            migrationBuilder.CreateTable(
                name: "Municipio",
                columns: table => new
                {
                    AtDate = table.Column<int>(type: "INTEGER", nullable: false),
                    IdMunicipio = table.Column<int>(type: "INTEGER", nullable: false),
                    IdProvincia = table.Column<int>(type: "INTEGER", nullable: false),
                    NombreMunicipio = table.Column<string>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipio", x => new { x.IdMunicipio, x.AtDate });
                });

            migrationBuilder.CreateTable(
                name: "ProductoPetrolifero",
                columns: table => new
                {
                    AtDate = table.Column<int>(type: "INTEGER", nullable: false),
                    IdProducto = table.Column<int>(type: "INTEGER", nullable: false),
                    NombreProducto = table.Column<string>(type: "TEXT", nullable: false),
                    NombreProductoAbreviatura = table.Column<string>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoPetrolifero", x => new { x.IdProducto, x.AtDate });
                });

            migrationBuilder.CreateTable(
                name: "Provincia",
                columns: table => new
                {
                    AtDate = table.Column<int>(type: "INTEGER", nullable: false),
                    IdProvincia = table.Column<int>(type: "INTEGER", nullable: false),
                    IdComunidadAutonoma = table.Column<int>(type: "INTEGER", nullable: false),
                    NombreProvincia = table.Column<string>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincia", x => new { x.IdProvincia, x.AtDate });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComunidadAutonoma");

            migrationBuilder.DropTable(
                name: "EstacionProductoPrecio");

            migrationBuilder.DropTable(
                name: "EstacionServicio");

            migrationBuilder.DropTable(
                name: "Municipio");

            migrationBuilder.DropTable(
                name: "ProductoPetrolifero");

            migrationBuilder.DropTable(
                name: "Provincia");
        }
    }
}
