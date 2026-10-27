using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToActivityPickBookmarkWheel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Wheels",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Bookmarks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ActivityPicks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Activities",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE \"Activities\" SET \"UserId\" = '00000000-0000-4000-8000-000000000001'");
            migrationBuilder.Sql("UPDATE \"ActivityPicks\" SET \"UserId\" = '00000000-0000-4000-8000-000000000001'");
            migrationBuilder.Sql("UPDATE \"Bookmarks\" SET \"UserId\" = '00000000-0000-4000-8000-000000000001'");
            migrationBuilder.Sql("UPDATE \"Wheels\" SET \"UserId\" = '00000000-0000-4000-8000-000000000001'");

            migrationBuilder.CreateIndex(
                name: "IX_Wheels_UserId",
                table: "Wheels",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookmarks_UserId",
                table: "Bookmarks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPicks_UserId",
                table: "ActivityPicks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_UserId",
                table: "Activities",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_AppUsers_UserId",
                table: "Activities",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityPicks_AppUsers_UserId",
                table: "ActivityPicks",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_AppUsers_UserId",
                table: "Bookmarks",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wheels_AppUsers_UserId",
                table: "Wheels",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_AppUsers_UserId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityPicks_AppUsers_UserId",
                table: "ActivityPicks");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_AppUsers_UserId",
                table: "Bookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_Wheels_AppUsers_UserId",
                table: "Wheels");

            migrationBuilder.DropIndex(
                name: "IX_Wheels_UserId",
                table: "Wheels");

            migrationBuilder.DropIndex(
                name: "IX_Bookmarks_UserId",
                table: "Bookmarks");

            migrationBuilder.DropIndex(
                name: "IX_ActivityPicks_UserId",
                table: "ActivityPicks");

            migrationBuilder.DropIndex(
                name: "IX_Activities_UserId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Wheels");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bookmarks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ActivityPicks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Activities");
        }
    }
}
