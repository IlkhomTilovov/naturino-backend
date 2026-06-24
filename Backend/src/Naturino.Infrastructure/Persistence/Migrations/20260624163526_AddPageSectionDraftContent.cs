using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Naturino.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPageSectionDraftContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DraftContent",
                table: "PageSections",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DraftContent",
                table: "PageSections");
        }
    }
}
