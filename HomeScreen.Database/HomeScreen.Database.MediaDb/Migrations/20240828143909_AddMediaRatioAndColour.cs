using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeScreen.Database.MediaDb.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaRatioAndColour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "BaseColourB",
                table: "MediaEntries",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "BaseColourG",
                table: "MediaEntries",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "BaseColourR",
                table: "MediaEntries",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<double>(
                name: "ImageRatio",
                table: "MediaEntries",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseColourB",
                table: "MediaEntries");

            migrationBuilder.DropColumn(
                name: "BaseColourG",
                table: "MediaEntries");

            migrationBuilder.DropColumn(
                name: "BaseColourR",
                table: "MediaEntries");

            migrationBuilder.DropColumn(
                name: "ImageRatio",
                table: "MediaEntries");
        }
    }
}
