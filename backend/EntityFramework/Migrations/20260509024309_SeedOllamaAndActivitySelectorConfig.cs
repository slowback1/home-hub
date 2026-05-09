using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class SeedOllamaAndActivitySelectorConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO "SystemConfigs" ("Id", "Namespace", "Key", "Value", "Type", "IsSecret")
                VALUES
                    ('ollama::url',        'ollama',   'url',       '',         'string', false),
                    ('activity::selector', 'activity', 'selector',  'random',   'string', false),
                    ('activity::ai_model', 'activity', 'ai_model',  'llama3.2', 'string', false)
                ON CONFLICT ("Id") DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "SystemConfigs"
                WHERE "Id" IN ('ollama::url', 'activity::selector', 'activity::ai_model');
                """);
        }
    }
}
