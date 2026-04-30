using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eg_travil.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToUserResponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UserResponses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserResponses_UserId",
                table: "UserResponses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserResponses_Users_UserId",
                table: "UserResponses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserResponses_Users_UserId",
                table: "UserResponses");

            migrationBuilder.DropIndex(
                name: "IX_UserResponses_UserId",
                table: "UserResponses");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserResponses");
        }
    }
}
