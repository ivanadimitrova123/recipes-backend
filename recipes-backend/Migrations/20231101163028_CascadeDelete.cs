using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace recipes_backend.Migrations
{
    public partial class CascadeDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Pictures_PictureId",
                table: "Recipes");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Pictures_PictureId",
                table: "Recipes",
                column: "PictureId",
                principalTable: "Pictures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Pictures_PictureId",
                table: "Recipes");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Pictures_PictureId",
                table: "Recipes",
                column: "PictureId",
                principalTable: "Pictures",
                principalColumn: "Id");
        }
    }
}
