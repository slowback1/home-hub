using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class SeedNbaScheduleFeatureFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FeatureFlags",
                columns: new[] { "Name", "IsEnabled" },
                values: new object[] { "NBA_SCHEDULE_ENABLED", false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "FeatureFlags", keyColumn: "Name", keyValue: "NBA_SCHEDULE_ENABLED");
        }
    }
}
