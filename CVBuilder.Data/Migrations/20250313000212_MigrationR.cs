using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVBuilder.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigrationR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Templates_TemplateId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Users_UserId",
                table: "Files");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_TemplateId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "Files");

            migrationBuilder.RenameTable(
                name: "Files",
                newName: "FileCVs");

            migrationBuilder.RenameColumn(
                name: "filePath",
                table: "FileCVs",
                newName: "FilePath");

            migrationBuilder.RenameIndex(
                name: "IX_Files_UserId",
                table: "FileCVs",
                newName: "IX_FileCVs_UserId");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "FileCVs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "FileCVs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Languages",
                table: "FileCVs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "FileCVs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "FileCVs",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "FileCVs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PathToCss",
                table: "FileCVs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "FileCVs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Skills",
                table: "FileCVs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "FileCVs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileCVs",
                table: "FileCVs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileCVs_Users_UserId",
                table: "FileCVs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileCVs_Users_UserId",
                table: "FileCVs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileCVs",
                table: "FileCVs");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "FileCVs");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "FileCVs");

            migrationBuilder.DropColumn(
                name: "Languages",
                table: "FileCVs");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "FileCVs");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "FileCVs");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "FileCVs");

            migrationBuilder.DropColumn(
                name: "PathToCss",
                table: "FileCVs");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "FileCVs");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "FileCVs");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "FileCVs");

            migrationBuilder.RenameTable(
                name: "FileCVs",
                newName: "Files");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "Files",
                newName: "filePath");

            migrationBuilder.RenameIndex(
                name: "IX_FileCVs_UserId",
                table: "Files",
                newName: "IX_Files_UserId");

            migrationBuilder.AddColumn<int>(
                name: "TemplateId",
                table: "Files",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Files_TemplateId",
                table: "Files",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Templates_TemplateId",
                table: "Files",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Users_UserId",
                table: "Files",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
