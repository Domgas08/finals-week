using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalsWeek.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "SessionQuestions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Questions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "SessionQuestions");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Questions");
        }
    }
}
