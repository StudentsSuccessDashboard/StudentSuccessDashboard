using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentSuccessDashboard.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudySessionAndAssignmentModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Topic",
                table: "StudySessions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "StudySessions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "StudySessions",
                type: "text",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Assignments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "PointsPossible",
                table: "Assignments",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Assignments",
                type: "text",
                nullable: false,
                defaultValue: "Not Started");

            migrationBuilder.CreateIndex(
                name: "IX_StudySessions_UserId",
                table: "StudySessions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudySessions_AspNetUsers_UserId",
                table: "StudySessions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudySessions_AspNetUsers_UserId",
                table: "StudySessions");

            migrationBuilder.DropIndex(
                name: "IX_StudySessions_UserId",
                table: "StudySessions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StudySessions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "PointsPossible",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Assignments");

            migrationBuilder.AlterColumn<string>(
                name: "Topic",
                table: "StudySessions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "StudySessions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}