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
                    PaisId = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 3, nullable: false),
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
                    ProvinciaId = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 3, nullable: false),
                    ProvinciaDenominacion = table.Column<string>(type: "TEXT", nullable: false),
                    PaisId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincia", x => x.ProvinciaId);
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
                    LocalidadId = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 3, nullable: false),
                    LocalidadDenominacion = table.Column<string>(type: "TEXT", nullable: false),
                    PaisId = table.Column<string>(type: "TEXT", nullable: false),
                    ProvinciaId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localidad", x => x.LocalidadId);
                    table.ForeignKey(
                        name: "FK_Localidad_PaisId",
                        column: x => x.PaisId,
                        principalTable: "Pais",
                        principalColumn: "PaisId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Localidad_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalTable: "Provincia",
                        principalColumn: "ProvinciaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Unidad",
                columns: table => new
                {
                    UnidadId = table.Column<int>(type: "INTEGER", nullable: false),
                    UnidadDenominacion = table.Column<string>(type: "TEXT", nullable: false),
                    CentroDirectivoId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaisId = table.Column<string>(type: "TEXT", nullable: false),
                    ProvinciaId = table.Column<string>(type: "TEXT", nullable: false),
                    LocalidadId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unidad", x => x.UnidadId);
                    table.ForeignKey(
                        name: "FK_Unidad_CentroDirectivoId",
                        column: x => x.CentroDirectivoId,
                        principalTable: "CentroDirectivo",
                        principalColumn: "CentroDirectivoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Unidad_LocalidadId",
                        column: x => x.LocalidadId,
                        principalTable: "Localidad",
                        principalColumn: "LocalidadId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Unidad_PaisId",
                        column: x => x.PaisId,
                        principalTable: "Pais",
                        principalColumn: "PaisId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Unidad_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalTable: "Provincia",
                        principalColumn: "ProvinciaId",
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
                    PaisId = table.Column<string>(type: "TEXT", nullable: true),
                    ProvinciaId = table.Column<string>(type: "TEXT", nullable: true),
                    LocalidadId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puesto", x => x.PuestoId);
                    table.ForeignKey(
                        name: "FK_Puesto_LocalidadId",
                        column: x => x.LocalidadId,
                        principalTable: "Localidad",
                        principalColumn: "LocalidadId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Puesto_PaisId",
                        column: x => x.PaisId,
                        principalTable: "Pais",
                        principalColumn: "PaisId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Puesto_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalTable: "Provincia",
                        principalColumn: "ProvinciaId",
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
                name: "IX_Localidad_PaisId",
                table: "Localidad",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_Localidad_ProvinciaId",
                table: "Localidad",
                column: "ProvinciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Provincia_PaisId",
                table: "Provincia",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_Puesto_LocalidadId",
                table: "Puesto",
                column: "LocalidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Puesto_PaisId",
                table: "Puesto",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_Puesto_ProvinciaId",
                table: "Puesto",
                column: "ProvinciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Puesto_UnidadId",
                table: "Puesto",
                column: "UnidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Unidad_CentroDirectivoId",
                table: "Unidad",
                column: "CentroDirectivoId");

            migrationBuilder.CreateIndex(
                name: "IX_Unidad_LocalidadId",
                table: "Unidad",
                column: "LocalidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Unidad_PaisId",
                table: "Unidad",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_Unidad_ProvinciaId",
                table: "Unidad",
                column: "ProvinciaId");
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
