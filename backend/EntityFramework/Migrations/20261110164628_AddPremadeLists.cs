using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddPremadeLists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PremadeLists",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PremadeLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PremadeLists_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PremadeListItems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PremadeListId = table.Column<string>(type: "text", nullable: false),
                    SavedItemId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PremadeListItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PremadeListItems_PremadeLists_PremadeListId",
                        column: x => x.PremadeListId,
                        principalTable: "PremadeLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PremadeListItems_PremadeListId",
                table: "PremadeListItems",
                column: "PremadeListId");

            migrationBuilder.CreateIndex(
                name: "IX_PremadeLists_UserId",
                table: "PremadeLists",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PremadeListItems");

            migrationBuilder.DropTable(
                name: "PremadeLists");
        }
    }
}
