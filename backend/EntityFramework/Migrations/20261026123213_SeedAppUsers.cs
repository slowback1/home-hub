using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class SeedAppUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "Name", "Password" },
                values: new object[,]
                {
                    { "00000000-0000-4000-8000-000000000001", "Alex", null },
                    { "00000000-0000-4000-8000-000000000002", "Sam", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "AppUsers", keyColumn: "Id", keyValue: "00000000-0000-4000-8000-000000000001");
            migrationBuilder.DeleteData(table: "AppUsers", keyColumn: "Id", keyValue: "00000000-0000-4000-8000-000000000002");
        }
    }
}
