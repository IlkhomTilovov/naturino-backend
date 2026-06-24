using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Naturino.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddThemes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Themes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "character varying(170)", maxLength: 170, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsDarkMode = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    FontHeading = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FontBody = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ColorTokensJson = table.Column<string>(type: "jsonb", nullable: false),
                    TypographyTokensJson = table.Column<string>(type: "jsonb", nullable: false),
                    RadiusTokensJson = table.Column<string>(type: "jsonb", nullable: false),
                    ShadowTokensJson = table.Column<string>(type: "jsonb", nullable: false),
                    ButtonTokensJson = table.Column<string>(type: "jsonb", nullable: false),
                    BrandingTokensJson = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Themes_Slug",
                table: "Themes",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Themes");
        }
    }
}
