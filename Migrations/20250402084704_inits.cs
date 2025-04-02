using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AskHire_Backend.Migrations
{
    /// <inheritdoc />
    public partial class inits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_JobRoles_JobId",
                table: "Vacancies");

            migrationBuilder.AlterColumn<Guid>(
                name: "JobId",
                table: "Vacancies",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "Instructions",
                table: "Vacancies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VacancyName",
                table: "Vacancies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "QuestionName",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Interview_Instructions",
                table: "Interviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_JobRoles_JobId",
                table: "Vacancies",
                column: "JobId",
                principalTable: "JobRoles",
                principalColumn: "JobId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_JobRoles_JobId",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "Instructions",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "VacancyName",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "QuestionName",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Interview_Instructions",
                table: "Interviews");

            migrationBuilder.AlterColumn<Guid>(
                name: "JobId",
                table: "Vacancies",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_JobRoles_JobId",
                table: "Vacancies",
                column: "JobId",
                principalTable: "JobRoles",
                principalColumn: "JobId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
