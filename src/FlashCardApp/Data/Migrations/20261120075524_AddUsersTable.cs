using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashCardApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Core",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "UUID", nullable: false),
                    username = table.Column<string>(type: "TEXT", nullable: false),
                    email = table.Column<string>(type: "TEXT", nullable: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                    table.CheckConstraint("CK_User_Email_Has_AtLeast_One_Characters_Before_At", "position('@' in email) > 1");
                    table.CheckConstraint("CK_User_Username_Length_GreaterThanZero", "length(btrim(username)) > 0");
                });

            migrationBuilder.Sql("CREATE UNIQUE INDEX uq_app_user_username ON \"Core\".\"Users\" (lower(username))");
            migrationBuilder.Sql("CREATE UNIQUE INDEX uq_app_user_email ON \"Core\".\"Users\" (lower(email))");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users",
                schema: "Core");
            migrationBuilder.Sql("DROP INDEX IF EXISTS uq_app_user_username");
            migrationBuilder.Sql("DROP INDEX IF EXISTS uq_app_user_email");
        }
    }
}
