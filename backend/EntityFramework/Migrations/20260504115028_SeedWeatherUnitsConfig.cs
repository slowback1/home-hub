using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class SeedWeatherUnitsConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO "SystemConfigs" ("Id", "Namespace", "Key", "Value", "Type", "IsSecret")
                VALUES ('weather::units', 'weather', 'units', 'imperial', 'select', false)
                ON CONFLICT ("Id") DO NOTHING;
                """);

            migrationBuilder.Sql("""
                INSERT INTO "SystemConfigOptions" ("SystemConfigId", "Value", "Label")
                VALUES
                    ('weather::units', 'imperial', 'Imperial (°F, mph)'),
                    ('weather::units', 'metric',   'Metric (°C, km/h)')
                ON CONFLICT DO NOTHING;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "SystemConfigOptions" WHERE "SystemConfigId" = 'weather::units';
                DELETE FROM "SystemConfigs" WHERE "Id" = 'weather::units';
                """);
        }
    }
}
