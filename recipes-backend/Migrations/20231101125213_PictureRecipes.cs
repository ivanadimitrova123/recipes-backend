using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace recipes_backend.Migrations
{
    public partial class PictureRecipes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoContentType",
                table: "Recipes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PhotoData",
                table: "Recipes",
                type: "bytea",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoContentType",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "PhotoData",
                table: "Recipes");
        }
    }
}
