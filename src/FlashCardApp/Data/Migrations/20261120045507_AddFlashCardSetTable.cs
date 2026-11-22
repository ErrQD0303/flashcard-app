using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashCardApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFlashCardSetTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Core");

            migrationBuilder.CreateTable(
                name: "FlashCardSets",
                schema: "Core",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "UUID", nullable: false),
                    title = table.Column<string>(type: "TEXT", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: false),
                    visibility = table.Column<string>(type: "TEXT", nullable: false),
                    source_set_id = table.Column<Guid>(type: "UUID", nullable: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlashCardSets", x => x.id);
                    table.CheckConstraint("CK_FlashCardSet_Title_Length_GreaterThanZero", "length(btrim(title)) > 0");
                    table.ForeignKey(
                        name: "FK_FlashCardSets_FlashCardSets_source_set_id",
                        column: x => x.source_set_id,
                        principalSchema: "Core",
                        principalTable: "FlashCardSets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlashCardSets_source_set_id",
                schema: "Core",
                table: "FlashCardSets",
                column: "source_set_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlashCardSets",
                schema: "Core");
        }
    }
}
