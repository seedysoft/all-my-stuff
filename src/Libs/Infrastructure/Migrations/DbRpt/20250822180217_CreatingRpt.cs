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
                name: "CentroDirectivo",
                columns: table => new
                {
                    CentroDirectivoId = table.Column<int>(type: "INTEGER", nullable: false),
                    CentroDirectivoDenominacion = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CentroDirectivo", x => x.CentroDirectivoId);
                });

            migrationBuilder.CreateTable(
                name: "Localidad",
                columns: table => new
                {
                    LocalidadId = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 3, nullable: false),
                    LocalidadDenominacion = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localidad", x => x.LocalidadId);
                });

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
                    PaisId = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 3, nullable: false),
                    PaisDenominacion = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pais", x => x.PaisId);
                });

            migrationBuilder.CreateTable(
                name: "Provincia",
                columns: table => new
                {
                    ProvinciaId = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 3, nullable: false),
                    ProvinciaDenominacion = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincia", x => x.ProvinciaId);
                });

            migrationBuilder.CreateTable(
                name: "Unidad",
                columns: table => new
                {
                    UnidadId = table.Column<int>(type: "INTEGER", nullable: false),
                    UnidadDenominacion = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unidad", x => x.UnidadId);
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
                    MinisterioId = table.Column<int>(type: "INTEGER", nullable: false),
                    CentroDirectivoId = table.Column<int>(type: "INTEGER", nullable: false),
                    UnidadId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaisId = table.Column<string>(type: "TEXT", nullable: true),
                    ProvinciaId = table.Column<string>(type: "TEXT", nullable: true),
                    LocalidadId = table.Column<string>(type: "TEXT", nullable: true),
                    ResidenciaPaisId = table.Column<string>(type: "TEXT", nullable: true),
                    ResidenciaProvinciaId = table.Column<string>(type: "TEXT", nullable: true),
                    ResidenciaLocalidadId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puesto", x => x.PuestoId);
                    table.ForeignKey(
                        name: "FK_Puesto_CentroDirectivo_CentroDirectivoId",
                        column: x => x.CentroDirectivoId,
                        principalTable: "CentroDirectivo",
                        principalColumn: "CentroDirectivoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Puesto_Localidad_LocalidadId",
                        column: x => x.LocalidadId,
                        principalTable: "Localidad",
                        principalColumn: "LocalidadId");
                    table.ForeignKey(
                        name: "FK_Puesto_Localidad_ResidenciaLocalidadId",
                        column: x => x.ResidenciaLocalidadId,
                        principalTable: "Localidad",
                        principalColumn: "LocalidadId");
                    table.ForeignKey(
                        name: "FK_Puesto_Ministerio_MinisterioId",
                        column: x => x.MinisterioId,
                        principalTable: "Ministerio",
                        principalColumn: "MinisterioId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Puesto_Pais_PaisId",
                        column: x => x.PaisId,
                        principalTable: "Pais",
                        principalColumn: "PaisId");
                    table.ForeignKey(
                        name: "FK_Puesto_Pais_ResidenciaPaisId",
                        column: x => x.ResidenciaPaisId,
                        principalTable: "Pais",
                        principalColumn: "PaisId");
                    table.ForeignKey(
                        name: "FK_Puesto_Provincia_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalTable: "Provincia",
                        principalColumn: "ProvinciaId");
                    table.ForeignKey(
                        name: "FK_Puesto_Provincia_ResidenciaProvinciaId",
                        column: x => x.ResidenciaProvinciaId,
                        principalTable: "Provincia",
                        principalColumn: "ProvinciaId");
                    table.ForeignKey(
                        name: "FK_Puesto_Unidad_UnidadId",
                        column: x => x.UnidadId,
                        principalTable: "Unidad",
                        principalColumn: "UnidadId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Puesto_CentroDirectivoId",
                table: "Puesto",
                column: "CentroDirectivoId");

            migrationBuilder.CreateIndex(
                name: "IX_Puesto_LocalidadId",
                table: "Puesto",
                column: "LocalidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Puesto_MinisterioId",
                table: "Puesto",
                column: "MinisterioId");

            migrationBuilder.CreateIndex(
                name: "IX_Puesto_PaisId",
                table: "Puesto",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_Puesto_ProvinciaId",
                table: "Puesto",
                column: "ProvinciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Puesto_ResidenciaLocalidadId",
                table: "Puesto",
                column: "ResidenciaLocalidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Puesto_ResidenciaPaisId",
                table: "Puesto",
                column: "ResidenciaPaisId");

            migrationBuilder.CreateIndex(
                name: "IX_Puesto_ResidenciaProvinciaId",
                table: "Puesto",
                column: "ResidenciaProvinciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Puesto_UnidadId",
                table: "Puesto",
                column: "UnidadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Puesto");

            migrationBuilder.DropTable(
                name: "CentroDirectivo");

            migrationBuilder.DropTable(
                name: "Localidad");

            migrationBuilder.DropTable(
                name: "Ministerio");

            migrationBuilder.DropTable(
                name: "Pais");

            migrationBuilder.DropTable(
                name: "Provincia");

            migrationBuilder.DropTable(
                name: "Unidad");
        }
    }
}
