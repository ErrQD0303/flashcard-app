using System.Reflection;
using System.Runtime.Serialization;
using FlashCardApp.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashCardApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEnumTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var flashCardSetVisibilities = typeof(FlashCardSetVisibility)
                .GetFields(BindingFlags.Public | BindingFlags.Static) // Get all public static fields (enum members)
                .Select(f => f.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? f.Name.ToLower())
                .ToList();
            migrationBuilder.Sql($"CREATE TYPE set_visibility AS ENUM ('{string.Join("', '", flashCardSetVisibilities)}');");

            var reviewResults = typeof(ReviewResult)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(f => f.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? f.Name.ToLower())
                .ToList();
            migrationBuilder.Sql($"CREATE TYPE review_result AS ENUM ('{string.Join("', '", reviewResults)}');");

            var studyModes = typeof(StudyMode)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(f => f.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? f.Name.ToLower())
                .ToList();
            migrationBuilder.Sql($"CREATE TYPE study_mode AS ENUM ('{string.Join("', '", studyModes)}');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TYPE IF EXISTS set_visibility;");
            migrationBuilder.Sql("DROP TYPE IF EXISTS review_result;");
            migrationBuilder.Sql("DROP TYPE IF EXISTS study_mode;");        }
    }
}
