using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QAProphet.Migrations
{
    /// <inheritdoc />
    public partial class QuestionUpdateField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateTime",
                table: "Questions",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdateTime",
                table: "Questions");
        }
    }
}
