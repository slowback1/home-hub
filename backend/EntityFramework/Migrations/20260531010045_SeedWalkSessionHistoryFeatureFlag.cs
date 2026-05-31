using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class SeedWalkSessionHistoryFeatureFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FeatureFlags",
                columns: new[] { "Name", "IsEnabled" },
                values: new object[] { "WALK_SESSION_HISTORY_ENABLED", false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "FeatureFlags", keyColumn: "Name", keyValue: "WALK_SESSION_HISTORY_ENABLED");
        }
    }
}
