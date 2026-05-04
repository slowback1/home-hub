using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemConfigOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemConfigOptions",
                columns: table => new
                {
                    SystemConfigId = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfigOptions", x => new { x.SystemConfigId, x.Value });
                    table.ForeignKey(
                        name: "FK_SystemConfigOptions_SystemConfigs_SystemConfigId",
                        column: x => x.SystemConfigId,
                        principalTable: "SystemConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Update Type on existing weather rows
            migrationBuilder.Sql("""
                UPDATE "SystemConfigs" SET "Type" = 'text'   WHERE "Id" = 'weather::zip_code';
                UPDATE "SystemConfigs" SET "Type" = 'secret' WHERE "Id" = 'weather::api_key';
                """);

            // Seed weather::provider as a select-type entry
            migrationBuilder.Sql("""
                INSERT INTO "SystemConfigs" ("Id", "Namespace", "Key", "Value", "Type", "IsSecret")
                VALUES ('weather::provider', 'weather', 'provider', 'mock', 'select', false)
                ON CONFLICT ("Id") DO NOTHING;
                """);

            // Seed options for weather::provider
            migrationBuilder.Sql("""
                INSERT INTO "SystemConfigOptions" ("SystemConfigId", "Value", "Label")
                VALUES
                    ('weather::provider', 'mock',            'Mock'),
                    ('weather::provider', 'openweathermap', 'Open Weather Map')
                ON CONFLICT DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "SystemConfigOptions" WHERE "SystemConfigId" = 'weather::provider';
                DELETE FROM "SystemConfigs" WHERE "Id" = 'weather::provider';
                UPDATE "SystemConfigs" SET "Type" = '' WHERE "Id" IN ('weather::zip_code', 'weather::api_key');
                """);

            migrationBuilder.DropTable(
                name: "SystemConfigOptions");
        }
    }
}
