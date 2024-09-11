using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeScreen.Database.MediaDb.Migrations
{
    /// <inheritdoc />
    public partial class AddImageAspectWidthHeight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageRatio",
                table: "MediaEntries",
                newName: "ImageRatioWidth");

            migrationBuilder.AddColumn<double>(
                name: "ImageRatioHeight",
                table: "MediaEntries",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageRatioHeight",
                table: "MediaEntries");

            migrationBuilder.RenameColumn(
                name: "ImageRatioWidth",
                table: "MediaEntries",
                newName: "ImageRatio");
        }
    }
}
