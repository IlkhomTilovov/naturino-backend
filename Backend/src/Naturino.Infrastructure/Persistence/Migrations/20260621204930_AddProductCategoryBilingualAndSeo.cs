using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Naturino.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductCategoryBilingualAndSeo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFollow",
                table: "ProductCategories",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsIndexable",
                table: "ProductCategories",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescriptionRu",
                table: "ProductCategories",
                type: "character varying(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescriptionUz",
                table: "ProductCategories",
                type: "character varying(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeywords",
                table: "ProductCategories",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitleRu",
                table: "ProductCategories",
                type: "character varying(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitleUz",
                table: "ProductCategories",
                type: "character varying(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "ProductCategories",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_SortOrder",
                table: "ProductCategories",
                column: "SortOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductCategories_SortOrder",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "IsFollow",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "IsIndexable",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "MetaDescriptionRu",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "MetaDescriptionUz",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "MetaKeywords",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "MetaTitleRu",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "MetaTitleUz",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "ProductCategories");
        }
    }
}
