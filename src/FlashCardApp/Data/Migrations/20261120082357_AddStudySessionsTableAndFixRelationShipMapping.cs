using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashCardApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStudySessionsTableAndFixRelationShipMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. If the column currently has a string/integer DEFAULT value, drop it first:
            migrationBuilder.Sql(
                @"ALTER TABLE ""Core"".""FlashCardSets"" ALTER COLUMN visibility DROP DEFAULT;");

            // 2. Alter the column type with an explicit USING cast:
            migrationBuilder.Sql(
                @"ALTER TABLE ""Core"".""FlashCardSets"" 
                    ALTER COLUMN visibility TYPE set_visibility 
                    USING visibility::text::set_visibility;");

            migrationBuilder.AddColumn<Guid>(
                name: "owner_id",
                schema: "Core",
                table: "FlashCardSets",
                type: "UUID",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "StudySessions",
                schema: "Core",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "UUID", nullable: false),
                    user_id = table.Column<Guid>(type: "UUID", nullable: false),
                    set_id = table.Column<Guid>(type: "UUID", nullable: false),
                    mode = table.Column<int>(type: "study_mode", nullable: false),
                    started_at = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false, defaultValueSql: "now()"),
                    ended_at = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySessions", x => x.id);
                    table.CheckConstraint("CK_StudySession_StartedAt_Before_Or_Equal_To_EndedAt", "ended_at >= started_at");
                    table.ForeignKey(
                        name: "FK_StudySessions_FlashCardSets_set_id",
                        column: x => x.set_id,
                        principalSchema: "Core",
                        principalTable: "FlashCardSets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudySessions_Users_user_id",
                        column: x => x.user_id,
                        principalSchema: "Core",
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlashCardSets_owner_id",
                schema: "Core",
                table: "FlashCardSets",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_StudySessions_set_id",
                schema: "Core",
                table: "StudySessions",
                column: "set_id");

            migrationBuilder.CreateIndex(
                name: "IX_StudySessions_user_id",
                schema: "Core",
                table: "StudySessions",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_FlashCardSets_Users_owner_id",
                schema: "Core",
                table: "FlashCardSets",
                column: "owner_id",
                principalSchema: "Core",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlashCardSets_Users_owner_id",
                schema: "Core",
                table: "FlashCardSets");

            migrationBuilder.DropTable(
                name: "StudySessions",
                schema: "Core");

            migrationBuilder.DropIndex(
                name: "IX_FlashCardSets_owner_id",
                schema: "Core",
                table: "FlashCardSets");

            migrationBuilder.DropColumn(
                name: "owner_id",
                schema: "Core",
                table: "FlashCardSets");

            migrationBuilder.Sql(
                @"ALTER TABLE ""Core"".""FlashCardSets"" 
                  ALTER COLUMN visibility TYPE TEXT 
                  USING visibility::text;");
        }
    }
}
