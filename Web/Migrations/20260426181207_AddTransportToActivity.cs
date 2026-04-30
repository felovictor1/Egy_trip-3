using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eg_travil.Migrations
{
    /// <inheritdoc />
    public partial class AddTransportToActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Transport",
                table: "SavedTripActivities",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Transport",
                table: "SavedTripActivities");
        }
    }
}
