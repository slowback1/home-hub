using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class SeedWeatherConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO "SystemConfigs" ("Id", "Namespace", "Key", "Value", "Type", "IsSecret")
                VALUES
                    ('weather::zip_code', 'weather', 'zip_code', '10001', '', false),
                    ('weather::api_key',  'weather', 'api_key',  '',      '', true)
                ON CONFLICT ("Id") DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "SystemConfigs"
                WHERE "Id" IN ('weather::zip_code', 'weather::api_key');
                """);
        }
    }
}
