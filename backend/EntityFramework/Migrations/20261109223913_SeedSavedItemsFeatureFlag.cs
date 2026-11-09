using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class SeedSavedItemsFeatureFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FeatureFlags",
                columns: new[] { "Name", "IsEnabled" },
                values: new object[] { "SAVED_ITEMS_ENABLED", false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "FeatureFlags", keyColumn: "Name", keyValue: "SAVED_ITEMS_ENABLED");
        }
    }
}
