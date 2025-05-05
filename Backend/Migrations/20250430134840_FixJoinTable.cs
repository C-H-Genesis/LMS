using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Migrations
{
    /// <inheritdoc />
    public partial class FixJoinTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "WrittenSubmission",
                table: "Submission",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId",
                table: "Assignment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "WrittenAssignment",
                table: "Assignment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_TeacherId",
                table: "Assignment",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_Users_TeacherId",
                table: "Assignment",
                column: "TeacherId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignment_Users_TeacherId",
                table: "Assignment");

            migrationBuilder.DropIndex(
                name: "IX_Assignment_TeacherId",
                table: "Assignment");

            migrationBuilder.DropColumn(
                name: "WrittenSubmission",
                table: "Submission");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Assignment");

            migrationBuilder.DropColumn(
                name: "WrittenAssignment",
                table: "Assignment");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
