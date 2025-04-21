using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QAProphet.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnecessaryField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerLike_Answers_AnswerId",
                table: "AnswerLike");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnswerLike",
                table: "AnswerLike");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "QuestionComments");

            migrationBuilder.DropColumn(
                name: "Dislikes",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "AnswerComments");

            migrationBuilder.RenameTable(
                name: "AnswerLike",
                newName: "AnswerLikes");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerLike_AnswerId",
                table: "AnswerLikes",
                newName: "IX_AnswerLikes_AnswerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnswerLikes",
                table: "AnswerLikes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerLikes_Answers_AnswerId",
                table: "AnswerLikes",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerLikes_Answers_AnswerId",
                table: "AnswerLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnswerLikes",
                table: "AnswerLikes");

            migrationBuilder.RenameTable(
                name: "AnswerLikes",
                newName: "AnswerLike");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerLikes_AnswerId",
                table: "AnswerLike",
                newName: "IX_AnswerLike_AnswerId");

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "QuestionComments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Dislikes",
                table: "Answers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "AnswerComments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnswerLike",
                table: "AnswerLike",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerLike_Answers_AnswerId",
                table: "AnswerLike",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
