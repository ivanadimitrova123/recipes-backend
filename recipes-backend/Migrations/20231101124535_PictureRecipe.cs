using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace recipes_backend.Migrations
{
    public partial class PictureRecipe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Picture_ProfilePictureId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Picture",
                table: "Picture");

            migrationBuilder.DropColumn(
                name: "Picture",
                table: "Recipes");

            migrationBuilder.RenameTable(
                name: "Picture",
                newName: "Pictures");

            migrationBuilder.AlterColumn<long>(
                name: "ProfilePictureId",
                table: "Users",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "PictureId",
                table: "Recipes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pictures",
                table: "Pictures",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_PictureId",
                table: "Recipes",
                column: "PictureId"
               // unique: true
                );

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Pictures_PictureId",
                table: "Recipes",
                column: "PictureId",
                principalTable: "Pictures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Pictures_ProfilePictureId",
                table: "Users",
                column: "ProfilePictureId",
                principalTable: "Pictures",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Pictures_PictureId",
                table: "Recipes");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Pictures_ProfilePictureId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_PictureId",
                table: "Recipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pictures",
                table: "Pictures");

            migrationBuilder.DropColumn(
                name: "PictureId",
                table: "Recipes");

            migrationBuilder.RenameTable(
                name: "Pictures",
                newName: "Picture");

            migrationBuilder.AlterColumn<long>(
                name: "ProfilePictureId",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Picture",
                table: "Recipes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Picture",
                table: "Picture",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Picture_ProfilePictureId",
                table: "Users",
                column: "ProfilePictureId",
                principalTable: "Picture",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
