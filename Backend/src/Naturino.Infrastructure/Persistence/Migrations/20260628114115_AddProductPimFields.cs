using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Naturino.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductPimFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Certifications",
                table: "Products",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExportInfo",
                table: "Products",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IngredientsList",
                table: "Products",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Certifications",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ExportInfo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IngredientsList",
                table: "Products");
        }
    }
}
