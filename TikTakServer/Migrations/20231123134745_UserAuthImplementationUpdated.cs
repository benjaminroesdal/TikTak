using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TikTakServer.Migrations
{
    /// <inheritdoc />
    public partial class UserAuthImplementationUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTokenDao_Users_UserId",
                table: "UserTokenDao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTokenDao",
                table: "UserTokenDao");

            migrationBuilder.RenameTable(
                name: "UserTokenDao",
                newName: "Tokens");

            migrationBuilder.RenameIndex(
                name: "IX_UserTokenDao_UserId",
                table: "Tokens",
                newName: "IX_Tokens_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tokens",
                table: "Tokens",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tokens_Users_UserId",
                table: "Tokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tokens_Users_UserId",
                table: "Tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tokens",
                table: "Tokens");

            migrationBuilder.RenameTable(
                name: "Tokens",
                newName: "UserTokenDao");

            migrationBuilder.RenameIndex(
                name: "IX_Tokens_UserId",
                table: "UserTokenDao",
                newName: "IX_UserTokenDao_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTokenDao",
                table: "UserTokenDao",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokenDao_Users_UserId",
                table: "UserTokenDao",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
