using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeScreen.Database.MediaDb.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedDirectionEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LatitudeDirection",
                table: "MediaEntries");

            migrationBuilder.DropColumn(
                name: "LongitudeDirection",
                table: "MediaEntries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LatitudeDirection",
                table: "MediaEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LongitudeDirection",
                table: "MediaEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
