using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seedysoft.Photo.Lib.Migrations;

/// <inheritdoc />
public partial class _20261112094411_Creating : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.CreateTable(
            name: "Photo",
            columns: static table => new
            {
                PhotoId = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                FilePath = table.Column<string>(type: "TEXT", nullable: false),
                Latitude = table.Column<double>(type: "REAL", nullable: true),
                Longitude = table.Column<double>(type: "REAL", nullable: true),
                Year = table.Column<int>(type: "INTEGER", nullable: false),
                PhotoGuid = table.Column<byte[]>(type: "BLOB", fixedLength: true, nullable: false)
            },
            constraints: static table => _ = table.PrimaryKey("PK_Photo", static x => x.PhotoId));

        _ = migrationBuilder.CreateTable(
            name: "Tag",
            columns: static table => new
            {
                TagId = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Text = table.Column<string>(type: "TEXT", nullable: false),
                TagGuid = table.Column<byte[]>(type: "BLOB", fixedLength: true, nullable: false)
            },
            constraints: static table => _ = table.PrimaryKey("PK_Tag", static x => x.TagId));

        _ = migrationBuilder.CreateTable(
            name: "PhotoTag",
            columns: static table => new
            {
                PhotoId = table.Column<long>(type: "INTEGER", nullable: false),
                TagId = table.Column<long>(type: "INTEGER", nullable: false)
            },
            constraints: static table =>
            {
                _ = table.PrimaryKey("PK_PhotoTag", static x => new { x.PhotoId, x.TagId });
                _ = table.ForeignKey(
                    name: "FK_PhotoTag_Photo_PhotoId",
                    column: static x => x.PhotoId,
                    principalTable: "Photo",
                    principalColumn: "PhotoId",
                    onDelete: ReferentialAction.Cascade);
                _ = table.ForeignKey(
                    name: "FK_PhotoTag_Tag_TagId",
                    column: static x => x.TagId,
                    principalTable: "Tag",
                    principalColumn: "TagId",
                    onDelete: ReferentialAction.Cascade);
            });

        _ = migrationBuilder.CreateIndex(
            name: "IX_PhotoTag_TagId",
            table: "PhotoTag",
            column: "TagId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropTable(
            name: "PhotoTag");

        _ = migrationBuilder.DropTable(
            name: "Photo");

        _ = migrationBuilder.DropTable(
            name: "Tag");
    }
}
