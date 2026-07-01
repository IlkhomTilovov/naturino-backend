using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Naturino.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificateExtendedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Certificates",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Certificates",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Scope",
                table: "Certificates",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TranslationsJson",
                table: "Certificates",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'{}'::jsonb");

            migrationBuilder.AddColumn<string>(
                name: "VerificationUrl",
                table: "Certificates",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "TranslationsJson",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "VerificationUrl",
                table: "Certificates");
        }
    }
}
