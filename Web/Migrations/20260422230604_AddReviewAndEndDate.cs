using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eg_travil.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewAndEndDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReviewRating",
                table: "SavedPlans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewText",
                table: "SavedPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "SavedPlans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TripEndDate",
                table: "SavedPlans",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewRating",
                table: "SavedPlans");

            migrationBuilder.DropColumn(
                name: "ReviewText",
                table: "SavedPlans");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "SavedPlans");

            migrationBuilder.DropColumn(
                name: "TripEndDate",
                table: "SavedPlans");
        }
    }
}
