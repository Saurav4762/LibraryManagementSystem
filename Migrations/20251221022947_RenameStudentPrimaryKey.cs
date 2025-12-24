using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Practice_Project.Migrations
{
    /// <inheritdoc />
    public partial class RenameStudentPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fines_BookIssues_BookIssueId1",
                table: "Fines");

            migrationBuilder.DropIndex(
                name: "IX_Fines_BookIssueId1",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "BookIssueId1",
                table: "Fines");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Students",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "Books",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "BookIssues",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<decimal>(
                name: "FineAmount",
                table: "BookIssues",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Students",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Books",
                newName: "BookId");

            migrationBuilder.AddColumn<int>(
                name: "BookIssueId1",
                table: "Fines",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "BookIssues",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "FineAmount",
                table: "BookIssues",
                type: "numeric(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_BookIssueId1",
                table: "Fines",
                column: "BookIssueId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Fines_BookIssues_BookIssueId1",
                table: "Fines",
                column: "BookIssueId1",
                principalTable: "BookIssues",
                principalColumn: "Id");
        }
    }
}
