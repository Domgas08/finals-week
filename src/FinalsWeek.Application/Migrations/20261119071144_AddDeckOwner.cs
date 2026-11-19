using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalsWeek.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddDeckOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Decks",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Decks_OwnerId",
                table: "Decks",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Decks_AspNetUsers_OwnerId",
                table: "Decks",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Decks_AspNetUsers_OwnerId",
                table: "Decks");

            migrationBuilder.DropIndex(
                name: "IX_Decks_OwnerId",
                table: "Decks");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Decks");
        }
    }
}
