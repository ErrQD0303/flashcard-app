using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashCardApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCardReviewsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardReviews",
                schema: "Core",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "UUID", nullable: false),
                    session_id = table.Column<Guid>(type: "UUID", nullable: false),
                    card_id = table.Column<Guid>(type: "UUID", nullable: false),
                    result = table.Column<int>(type: "review_result", nullable: false),
                    response_time_ms = table.Column<int>(type: "INT", nullable: false),
                    reviewed_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardReviews", x => x.id);
                    table.CheckConstraint("CK_CardReview_ResponseTimeMs_NonNegative", "response_time_ms >= 0");
                    table.ForeignKey(
                        name: "FK_CardReviews_FlashCards_card_id",
                        column: x => x.card_id,
                        principalSchema: "Core",
                        principalTable: "FlashCards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardReviews_StudySessions_session_id",
                        column: x => x.session_id,
                        principalSchema: "Core",
                        principalTable: "StudySessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardReviews_card_id",
                schema: "Core",
                table: "CardReviews",
                column: "card_id");

            migrationBuilder.CreateIndex(
                name: "IX_CardReviews_session_id",
                schema: "Core",
                table: "CardReviews",
                column: "session_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardReviews",
                schema: "Core");
        }
    }
}
