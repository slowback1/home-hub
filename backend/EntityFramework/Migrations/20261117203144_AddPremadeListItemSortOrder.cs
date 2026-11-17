using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddPremadeListItemSortOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "PremadeListItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                """
                UPDATE "PremadeListItems" p
                SET "SortOrder" = sub.rn - 1
                FROM (
                    SELECT "Id", ROW_NUMBER() OVER (PARTITION BY "PremadeListId" ORDER BY "CreatedAt") AS rn
                    FROM "PremadeListItems"
                ) sub
                WHERE p."Id" = sub."Id";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "PremadeListItems");
        }
    }
}
