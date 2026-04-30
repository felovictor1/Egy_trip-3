using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eg_travil.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelDetailsToPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HotelDetails",
                table: "SavedPlans",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HotelDetails",
                table: "SavedPlans");
        }
    }
}
