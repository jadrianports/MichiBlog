using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MichiBlog.WebApp.Migrations
{
    /// <inheritdoc />
    public partial class ReactionUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Reactions");

            migrationBuilder.AddColumn<bool>(
                name: "Disliked",
                table: "Reactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Liked",
                table: "Reactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Reactions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_UserId",
                table: "Reactions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reactions_AspNetUsers_UserId",
                table: "Reactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reactions_AspNetUsers_UserId",
                table: "Reactions");

            migrationBuilder.DropIndex(
                name: "IX_Reactions_UserId",
                table: "Reactions");

            migrationBuilder.DropColumn(
                name: "Disliked",
                table: "Reactions");

            migrationBuilder.DropColumn(
                name: "Liked",
                table: "Reactions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Reactions");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Reactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
