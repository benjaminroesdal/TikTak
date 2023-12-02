using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TikTakServer.Migrations
{
    /// <inheritdoc />
    public partial class modifiedlikedbsetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Videos_UserId",
                table: "Likes");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_VideoId",
                table: "Likes",
                column: "VideoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Videos_VideoId",
                table: "Likes",
                column: "VideoId",
                principalTable: "Videos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Videos_VideoId",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Likes_VideoId",
                table: "Likes");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Videos_UserId",
                table: "Likes",
                column: "UserId",
                principalTable: "Videos",
                principalColumn: "Id");
        }
    }
}
