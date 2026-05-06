using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class SeedAudiobookFeatureFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FeatureFlags",
                columns: new[] { "Name", "IsEnabled" },
                values: new object[] { "AUDIOBOOK_ENABLED", false });

            migrationBuilder.Sql("""
                INSERT INTO "SystemConfigs" ("Id", "Namespace", "Key", "Value", "Type", "IsSecret")
                VALUES
                    ('audiobook::provider', 'audiobook', 'provider', 'mock',  'select', false),
                    ('audiobook::url',      'audiobook', 'url',      '',      'string', false),
                    ('audiobook::api_key',  'audiobook', 'api_key',  '',      'string', true)
                ON CONFLICT ("Id") DO NOTHING;
                """);

            migrationBuilder.Sql("""
                INSERT INTO "SystemConfigOptions" ("SystemConfigId", "Value", "Label")
                VALUES
                    ('audiobook::provider', 'mock',        'Mock'),
                    ('audiobook::provider', 'gpu-service', 'GPU Service')
                ON CONFLICT DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "FeatureFlags", keyColumn: "Name", keyValue: "AUDIOBOOK_ENABLED");

            migrationBuilder.Sql("""
                DELETE FROM "SystemConfigOptions" WHERE "SystemConfigId" = 'audiobook::provider';
                DELETE FROM "SystemConfigs" WHERE "Id" IN ('audiobook::provider', 'audiobook::url', 'audiobook::api_key');
                """);
        }
    }
}
