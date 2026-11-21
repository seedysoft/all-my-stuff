using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seedysoft.Photo.Lib.Migrations;

/// <inheritdoc />
public partial class _20261121120812_Creating : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Photo",
            columns: table => new
            {
                PhotoId = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                FilePath = table.Column<string>(type: "TEXT", nullable: false),
                Latitude = table.Column<double>(type: "REAL", nullable: true),
                Longitude = table.Column<double>(type: "REAL", nullable: true),
                Year = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Photo", x => x.PhotoId);
            });

        migrationBuilder.CreateTable(
            name: "Tag",
            columns: table => new
            {
                TagId = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Text = table.Column<string>(type: "TEXT", nullable: false),
                TagGuid = table.Column<byte[]>(type: "BLOB", fixedLength: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tag", x => x.TagId);
            });

        migrationBuilder.CreateTable(
            name: "PhotoTag",
            columns: table => new
            {
                PhotoId = table.Column<long>(type: "INTEGER", nullable: false),
                TagId = table.Column<long>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PhotoTag", x => new { x.PhotoId, x.TagId });
                table.ForeignKey(
                    name: "FK_PhotoTag_Photo_PhotoId",
                    column: x => x.PhotoId,
                    principalTable: "Photo",
                    principalColumn: "PhotoId",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_PhotoTag_Tag_TagId",
                    column: x => x.TagId,
                    principalTable: "Tag",
                    principalColumn: "TagId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_PhotoTag_TagId",
            table: "PhotoTag",
            column: "TagId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "PhotoTag");

        migrationBuilder.DropTable(
            name: "Photo");

        migrationBuilder.DropTable(
            name: "Tag");
    }
}
