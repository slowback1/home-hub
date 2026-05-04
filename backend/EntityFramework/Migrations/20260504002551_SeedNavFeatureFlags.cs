using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class SeedNavFeatureFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FeatureFlags",
                columns: new[] { "Name", "IsEnabled" },
                values: new object[,]
                {
                    { "CHORE_TASK_TRACKER_ENABLED", true },
                    { "ACTIVITY_PICKER_ENABLED", true },
                    { "RETRO_ACHIEVEMENTS_ENABLED", true },
                    { "WEATHER_ENABLED", true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "FeatureFlags", keyColumn: "Name", keyValue: "CHORE_TASK_TRACKER_ENABLED");
            migrationBuilder.DeleteData(table: "FeatureFlags", keyColumn: "Name", keyValue: "ACTIVITY_PICKER_ENABLED");
            migrationBuilder.DeleteData(table: "FeatureFlags", keyColumn: "Name", keyValue: "RETRO_ACHIEVEMENTS_ENABLED");
            migrationBuilder.DeleteData(table: "FeatureFlags", keyColumn: "Name", keyValue: "WEATHER_ENABLED");
        }
    }
}
