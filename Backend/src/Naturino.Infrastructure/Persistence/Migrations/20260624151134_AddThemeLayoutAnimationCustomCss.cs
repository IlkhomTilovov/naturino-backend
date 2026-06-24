using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Naturino.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddThemeLayoutAnimationCustomCss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnimationTokensJson",
                table: "Themes",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AppearanceMode",
                table: "Themes",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomCss",
                table: "Themes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Themes",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LayoutTokensJson",
                table: "Themes",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "Themes",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnimationTokensJson",
                table: "Themes");

            migrationBuilder.DropColumn(
                name: "AppearanceMode",
                table: "Themes");

            migrationBuilder.DropColumn(
                name: "CustomCss",
                table: "Themes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Themes");

            migrationBuilder.DropColumn(
                name: "LayoutTokensJson",
                table: "Themes");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Themes");
        }
    }
}
