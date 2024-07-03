using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeScreen.Database.MediaDb.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MediaEntries",
                columns: table => new
                {
                    MediaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalFile = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    OriginalHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    LatitudeDirection = table.Column<int>(type: "int", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    LongitudeDirection = table.Column<int>(type: "int", nullable: false),
                    LocationLabel = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false),
                    CapturedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CapturedOffset = table.Column<TimeSpan>(type: "time", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", maxLength: 1048576, nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaEntries", x => x.MediaId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaEntries");
        }
    }
}
