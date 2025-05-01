using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QAProphet.Migrations
{
    /// <inheritdoc />
    public partial class ComplaintCategoriesTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerComplaint_Answers_AnswerId",
                table: "AnswerComplaint");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionComplaint_Questions_QuestionId",
                table: "QuestionComplaint");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionSubscribe_Questions_QuestionId",
                table: "QuestionSubscribe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionSubscribe",
                table: "QuestionSubscribe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionComplaint",
                table: "QuestionComplaint");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnswerComplaint",
                table: "AnswerComplaint");

            migrationBuilder.RenameTable(
                name: "QuestionSubscribe",
                newName: "QuestionSubscribes");

            migrationBuilder.RenameTable(
                name: "QuestionComplaint",
                newName: "QuestionComplaints");

            migrationBuilder.RenameTable(
                name: "AnswerComplaint",
                newName: "AnswerComplaints");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionSubscribe_QuestionId",
                table: "QuestionSubscribes",
                newName: "IX_QuestionSubscribes_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionComplaint_QuestionId",
                table: "QuestionComplaints",
                newName: "IX_QuestionComplaints_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerComplaint_AnswerId",
                table: "AnswerComplaints",
                newName: "IX_AnswerComplaints_AnswerId");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "QuestionComplaints",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "AnswerComplaints",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionSubscribes",
                table: "QuestionSubscribes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionComplaints",
                table: "QuestionComplaints",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnswerComplaints",
                table: "AnswerComplaints",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AnswerComplaintCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerComplaintCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionComplaintCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionComplaintCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionComplaints_CategoryId",
                table: "QuestionComplaints",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerComplaints_CategoryId",
                table: "AnswerComplaints",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerComplaints_AnswerComplaintCategories_CategoryId",
                table: "AnswerComplaints",
                column: "CategoryId",
                principalTable: "AnswerComplaintCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerComplaints_Answers_AnswerId",
                table: "AnswerComplaints",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionComplaints_QuestionComplaintCategories_CategoryId",
                table: "QuestionComplaints",
                column: "CategoryId",
                principalTable: "QuestionComplaintCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionComplaints_Questions_QuestionId",
                table: "QuestionComplaints",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionSubscribes_Questions_QuestionId",
                table: "QuestionSubscribes",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerComplaints_AnswerComplaintCategories_CategoryId",
                table: "AnswerComplaints");

            migrationBuilder.DropForeignKey(
                name: "FK_AnswerComplaints_Answers_AnswerId",
                table: "AnswerComplaints");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionComplaints_QuestionComplaintCategories_CategoryId",
                table: "QuestionComplaints");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionComplaints_Questions_QuestionId",
                table: "QuestionComplaints");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionSubscribes_Questions_QuestionId",
                table: "QuestionSubscribes");

            migrationBuilder.DropTable(
                name: "AnswerComplaintCategories");

            migrationBuilder.DropTable(
                name: "QuestionComplaintCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionSubscribes",
                table: "QuestionSubscribes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionComplaints",
                table: "QuestionComplaints");

            migrationBuilder.DropIndex(
                name: "IX_QuestionComplaints_CategoryId",
                table: "QuestionComplaints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnswerComplaints",
                table: "AnswerComplaints");

            migrationBuilder.DropIndex(
                name: "IX_AnswerComplaints_CategoryId",
                table: "AnswerComplaints");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "QuestionComplaints");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "AnswerComplaints");

            migrationBuilder.RenameTable(
                name: "QuestionSubscribes",
                newName: "QuestionSubscribe");

            migrationBuilder.RenameTable(
                name: "QuestionComplaints",
                newName: "QuestionComplaint");

            migrationBuilder.RenameTable(
                name: "AnswerComplaints",
                newName: "AnswerComplaint");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionSubscribes_QuestionId",
                table: "QuestionSubscribe",
                newName: "IX_QuestionSubscribe_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionComplaints_QuestionId",
                table: "QuestionComplaint",
                newName: "IX_QuestionComplaint_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerComplaints_AnswerId",
                table: "AnswerComplaint",
                newName: "IX_AnswerComplaint_AnswerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionSubscribe",
                table: "QuestionSubscribe",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionComplaint",
                table: "QuestionComplaint",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnswerComplaint",
                table: "AnswerComplaint",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerComplaint_Answers_AnswerId",
                table: "AnswerComplaint",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionComplaint_Questions_QuestionId",
                table: "QuestionComplaint",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionSubscribe_Questions_QuestionId",
                table: "QuestionSubscribe",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
