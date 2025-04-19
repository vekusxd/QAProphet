using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QAProphet.Migrations
{
    /// <inheritdoc />
    public partial class CommendAuthorNamField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorName",
                table: "QuestionComments",
                type: "character varying(96)",
                maxLength: 96,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuthorName",
                table: "AnswerComments",
                type: "character varying(96)",
                maxLength: 96,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorName",
                table: "QuestionComments");

            migrationBuilder.DropColumn(
                name: "AuthorName",
                table: "AnswerComments");
        }
    }
}
