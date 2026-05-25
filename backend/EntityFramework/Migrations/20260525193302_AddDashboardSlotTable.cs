using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddDashboardSlotTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DashboardSlots",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    LayoutFormat = table.Column<string>(type: "text", nullable: false),
                    SlotIndex = table.Column<int>(type: "integer", nullable: false),
                    WidgetType = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardSlots", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DashboardSlots_LayoutFormat_SlotIndex",
                table: "DashboardSlots",
                columns: new[] { "LayoutFormat", "SlotIndex" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DashboardSlots");
        }
    }
}
