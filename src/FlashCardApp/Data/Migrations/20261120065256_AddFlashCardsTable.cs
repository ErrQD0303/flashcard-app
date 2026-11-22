using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashCardApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFlashCardsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlashCardSets_FlashCardSets_source_set_id",
                schema: "Core",
                table: "FlashCardSets");

            migrationBuilder.CreateTable(
                name: "FlashCards",
                schema: "Core",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "UUID", nullable: false),
                    set_id = table.Column<Guid>(type: "UUID", nullable: false),
                    term = table.Column<string>(type: "TEXT", nullable: false),
                    definition = table.Column<string>(type: "TEXT", nullable: false),
                    position = table.Column<int>(type: "INT", nullable: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlashCards", x => x.id);
                    table.CheckConstraint("CK_FlashCard_Definition_Length_GreaterThanZero", "length(btrim(definition)) > 0");
                    table.CheckConstraint("CK_FlashCard_Position_GreaterThanZero", "position > 0");
                    table.CheckConstraint("CK_FlashCard_Term_Length_GreaterThanZero", "length(btrim(term)) > 0");
                    table.ForeignKey(
                        name: "FK_FlashCards_FlashCardSets_set_id",
                        column: x => x.set_id,
                        principalSchema: "Core",
                        principalTable: "FlashCardSets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlashCards_set_id",
                schema: "Core",
                table: "FlashCards",
                column: "set_id");

            migrationBuilder.AddForeignKey(
                name: "FK_FlashCardSets_FlashCardSets_source_set_id",
                schema: "Core",
                table: "FlashCardSets",
                column: "source_set_id",
                principalSchema: "Core",
                principalTable: "FlashCardSets",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.Sql("ALTER TABLE \"Core\".\"FlashCards\" ADD CONSTRAINT uq_card_position UNIQUE (set_id, position) DEFERRABLE INITIALLY DEFERRED;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlashCardSets_FlashCardSets_source_set_id",
                schema: "Core",
                table: "FlashCardSets");

            migrationBuilder.DropTable(
                name: "FlashCards",
                schema: "Core");

            migrationBuilder.AddForeignKey(
                name: "FK_FlashCardSets_FlashCardSets_source_set_id",
                schema: "Core",
                table: "FlashCardSets",
                column: "source_set_id",
                principalSchema: "Core",
                principalTable: "FlashCardSets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql("ALTER TABLE \"Core\".\"FlashCards\" DROP CONSTRAINT IF EXISTS uq_card_position;");
        }
    }
}
