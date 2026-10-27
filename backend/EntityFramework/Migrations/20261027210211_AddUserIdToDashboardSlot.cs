using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToDashboardSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DashboardSlots_LayoutFormat_SlotIndex",
                table: "DashboardSlots");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "DashboardSlots",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                "UPDATE \"DashboardSlots\" SET \"UserId\" = '00000000-0000-4000-8000-000000000001' WHERE \"UserId\" = '';");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardSlots_UserId_LayoutFormat_SlotIndex",
                table: "DashboardSlots",
                columns: new[] { "UserId", "LayoutFormat", "SlotIndex" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DashboardSlots_AppUsers_UserId",
                table: "DashboardSlots",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DashboardSlots_AppUsers_UserId",
                table: "DashboardSlots");

            migrationBuilder.DropIndex(
                name: "IX_DashboardSlots_UserId_LayoutFormat_SlotIndex",
                table: "DashboardSlots");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DashboardSlots");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardSlots_LayoutFormat_SlotIndex",
                table: "DashboardSlots",
                columns: new[] { "LayoutFormat", "SlotIndex" },
                unique: true);
        }
    }
}
