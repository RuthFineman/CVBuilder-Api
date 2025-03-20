using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVBuilder.Data.Migrations
{
    /// <inheritdoc />
    public partial class Mi7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CssFilePath",
                table: "Templates");

            migrationBuilder.DropColumn(
                name: "PathToCss",
                table: "FileCVs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CssFilePath",
                table: "Templates",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PathToCss",
                table: "FileCVs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
