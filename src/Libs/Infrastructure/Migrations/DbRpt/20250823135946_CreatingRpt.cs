using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seedysoft.Libs.Infrastructure.Migrations.DbRpt
{
    /// <inheritdoc />
    public partial class CreatingRpt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ministerio",
                columns: table => new
                {
                    MinisterioId = table.Column<int>(type: "INTEGER", nullable: false),
                    MinisterioDenominacion = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ministerio", x => x.MinisterioId);
                });

            migrationBuilder.CreateTable(
                name: "Pais",
                columns: table => new
                {
                    PaisId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaisDenominacion = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pais", x => x.PaisId);
                });

            migrationBuilder.CreateTable(
                name: "CentroDirectivo",
                columns: table => new
                {
                    CentroDirectivoId = table.Column<int>(type: "INTEGER", nullable: false),
                    CentroDirectivoDenominacion = table.Column<string>(type: "TEXT", nullable: false),
                    MinisterioId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CentroDirectivo", x => x.CentroDirectivoId);
                    table.ForeignKey(
                        name: "FK_CentroDirectivo_MinisterioId",
                        column: x => x.MinisterioId,
                        principalTable: "Ministerio",
                        principalColumn: "MinisterioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Provincia",
                columns: table => new
                {
                    ProvinciaId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaisId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProvinciaDenominacion = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincia", x => new { x.PaisId, x.ProvinciaId });
                    table.ForeignKey(
                        name: "FK_Provincia_PaisId",
                        column: x => x.PaisId,
                        principalTable: "Pais",
                        principalColumn: "PaisId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Localidad",
                columns: table => new
                {
                    LocalidadId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaisId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProvinciaId = table.Column<int>(type: "INTEGER", nullable: false),
                    LocalidadDenominacion = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localidad", x => new { x.PaisId, x.ProvinciaId, x.LocalidadId });
                    table.ForeignKey(
                        name: "FK_Localidad_PaisId",
                        column: x => x.PaisId,
                        principalTable: "Pais",
                        principalColumn: "PaisId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Localidad_Provincia",
                        columns: x => new { x.PaisId, x.ProvinciaId },
                        principalTable: "Provincia",
                        principalColumns: new[] { "PaisId", "ProvinciaId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Unidad",
                columns: table => new
                {
                    UnidadId = table.Column<int>(type: "INTEGER", nullable: false),
                    UnidadDenominacion = table.Column<string>(type: "TEXT", nullable: false),
                    CentroDirectivoId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaisId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProvinciaId = table.Column<int>(type: "INTEGER", nullable: false),
                    LocalidadId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unidad", x => x.UnidadId);
                    table.ForeignKey(
                        name: "FK_Puesto_PaisId",
                        column: x => x.PaisId,
                        principalTable: "Pais",
                        principalColumn: "PaisId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Puesto_Provincia",
                        columns: x => new { x.PaisId, x.ProvinciaId },
                        principalTable: "Provincia",
                        principalColumns: new[] { "PaisId", "ProvinciaId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Unidad_CentroDirectivoId",
                        column: x => x.CentroDirectivoId,
                        principalTable: "CentroDirectivo",
                        principalColumn: "CentroDirectivoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Unidad_Localidad",
                        columns: x => new { x.PaisId, x.ProvinciaId, x.LocalidadId },
                        principalTable: "Localidad",
                        principalColumns: new[] { "PaisId", "ProvinciaId", "LocalidadId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Puesto",
                columns: table => new
                {
                    PuestoId = table.Column<int>(type: "INTEGER", nullable: false),
                    PuestoDenominacionCorta = table.Column<string>(type: "TEXT", nullable: false),
                    PuestoDenominacionLarga = table.Column<string>(type: "TEXT", nullable: false),
                    Nivel = table.Column<int>(type: "INTEGER", nullable: false),
                    ComplementoEspecifico = table.Column<decimal>(type: "TEXT", precision: 2, nullable: false),
                    TipoPuesto = table.Column<string>(type: "TEXT", nullable: true),
                    Provision = table.Column<string>(type: "TEXT", nullable: true),
                    Adscripcion = table.Column<string>(type: "TEXT", nullable: true),
                    GrupoSubgrupo = table.Column<string>(type: "TEXT", nullable: true),
                    Cuerpo = table.Column<string>(type: "TEXT", nullable: true),
                    TitulacionAcademica = table.Column<string>(type: "TEXT", nullable: true),
                    FormacionEspecifica = table.Column<string>(type: "TEXT", nullable: true),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true),
                    Estado = table.Column<string>(type: "TEXT", nullable: false),
                    UnidadId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaisId = table.Column<int>(type: "INTEGER", nullable: true),
                    ProvinciaId = table.Column<int>(type: "INTEGER", nullable: true),
                    LocalidadId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puesto", x => x.PuestoId);
                    table.ForeignKey(
                        name: "FK_Puesto_Localidad",
                        columns: x => new { x.PaisId, x.ProvinciaId, x.LocalidadId },
                        principalTable: "Localidad",
                        principalColumns: new[] { "PaisId", "ProvinciaId", "LocalidadId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Puesto_PaisId",
                        column: x => x.PaisId,
                        principalTable: "Pais",
                        principalColumn: "PaisId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Puesto_Provincia",
                        columns: x => new { x.PaisId, x.ProvinciaId },
                        principalTable: "Provincia",
                        principalColumns: new[] { "PaisId", "ProvinciaId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Puesto_UnidadId",
                        column: x => x.UnidadId,
                        principalTable: "Unidad",
                        principalColumn: "UnidadId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CentroDirectivo_MinisterioId",
                table: "CentroDirectivo",
                column: "MinisterioId");

            migrationBuilder.CreateIndex(
                name: "IX_Puesto_PaisId_ProvinciaId_LocalidadId",
                table: "Puesto",
                columns: new[] { "PaisId", "ProvinciaId", "LocalidadId" });

            migrationBuilder.CreateIndex(
                name: "IX_Puesto_UnidadId",
                table: "Puesto",
                column: "UnidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Unidad_CentroDirectivoId",
                table: "Unidad",
                column: "CentroDirectivoId");

            migrationBuilder.CreateIndex(
                name: "IX_Unidad_PaisId_ProvinciaId_LocalidadId",
                table: "Unidad",
                columns: new[] { "PaisId", "ProvinciaId", "LocalidadId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Puesto");

            migrationBuilder.DropTable(
                name: "Unidad");

            migrationBuilder.DropTable(
                name: "CentroDirectivo");

            migrationBuilder.DropTable(
                name: "Localidad");

            migrationBuilder.DropTable(
                name: "Ministerio");

            migrationBuilder.DropTable(
                name: "Provincia");

            migrationBuilder.DropTable(
                name: "Pais");
        }
    }
}
