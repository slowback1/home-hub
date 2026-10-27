using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ChoreTasks",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChoreTasks_UserId",
                table: "ChoreTasks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChoreTasks_AppUsers_UserId",
                table: "ChoreTasks",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChoreTasks_AppUsers_UserId",
                table: "ChoreTasks");

            migrationBuilder.DropIndex(
                name: "IX_ChoreTasks_UserId",
                table: "ChoreTasks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ChoreTasks");
        }
    }
}
