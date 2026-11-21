using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class SeedNbaProviderConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO "SystemConfigs" ("Id", "Namespace", "Key", "Value", "Type", "IsSecret")
                VALUES ('nba::provider', 'nba', 'provider', 'official', 'select', false)
                ON CONFLICT ("Id") DO NOTHING;
                """);

            migrationBuilder.Sql("""
                INSERT INTO "SystemConfigOptions" ("SystemConfigId", "Value", "Label")
                VALUES
                    ('nba::provider', 'mock',     'Mock'),
                    ('nba::provider', 'official', 'Official NBA API')
                ON CONFLICT DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "SystemConfigOptions" WHERE "SystemConfigId" = 'nba::provider';
                DELETE FROM "SystemConfigs" WHERE "Id" = 'nba::provider';
                """);
        }
    }
}
