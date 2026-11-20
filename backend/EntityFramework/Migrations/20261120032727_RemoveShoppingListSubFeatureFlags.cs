using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class RemoveShoppingListSubFeatureFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "FeatureFlags", keyColumn: "Name", keyValue: "SAVED_ITEMS_ENABLED");
            migrationBuilder.DeleteData(table: "FeatureFlags", keyColumn: "Name", keyValue: "PREMADE_LISTS_ENABLED");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FeatureFlags",
                columns: new[] { "Name", "IsEnabled" },
                values: new object[] { "SAVED_ITEMS_ENABLED", false });
            migrationBuilder.InsertData(
                table: "FeatureFlags",
                columns: new[] { "Name", "IsEnabled" },
                values: new object[] { "PREMADE_LISTS_ENABLED", false });
        }
    }
}
