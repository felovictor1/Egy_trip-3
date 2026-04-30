using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eg_travil.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentFieldsToSavedPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HotelName",
                table: "SavedPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HotelPricePerNight",
                table: "SavedPlans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "SavedPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Nights",
                table: "SavedPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HotelName",
                table: "SavedPlans");

            migrationBuilder.DropColumn(
                name: "HotelPricePerNight",
                table: "SavedPlans");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "SavedPlans");

            migrationBuilder.DropColumn(
                name: "Nights",
                table: "SavedPlans");
        }
    }
}
