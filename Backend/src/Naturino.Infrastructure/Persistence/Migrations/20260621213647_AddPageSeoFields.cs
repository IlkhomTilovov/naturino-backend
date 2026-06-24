using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Naturino.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPageSeoFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFollow",
                table: "Pages",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsIndexable",
                table: "Pages",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Pages",
                type: "character varying(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeywords",
                table: "Pages",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitle",
                table: "Pages",
                type: "character varying(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OgImageFileId",
                table: "Pages",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pages_OgImageFileId",
                table: "Pages",
                column: "OgImageFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_MediaFiles_OgImageFileId",
                table: "Pages",
                column: "OgImageFileId",
                principalTable: "MediaFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pages_MediaFiles_OgImageFileId",
                table: "Pages");

            migrationBuilder.DropIndex(
                name: "IX_Pages_OgImageFileId",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "IsFollow",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "IsIndexable",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "MetaKeywords",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "MetaTitle",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "OgImageFileId",
                table: "Pages");
        }
    }
}
