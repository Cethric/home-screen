using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeScreen.Database.MediaDb.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOldEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [MediaEntries]", false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
