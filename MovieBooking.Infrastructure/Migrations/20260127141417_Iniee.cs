using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Iniee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "AdminRequests",
                newName: "RequestedBy");

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Screens",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Screens",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedBy",
                table: "AdminRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminRequests_RequestedBy",
                table: "AdminRequests",
                column: "RequestedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminRequests_Users_RequestedBy",
                table: "AdminRequests",
                column: "RequestedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminRequests_Users_RequestedBy",
                table: "AdminRequests");

            migrationBuilder.DropIndex(
                name: "IX_AdminRequests_RequestedBy",
                table: "AdminRequests");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Screens");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Screens");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                table: "AdminRequests");

            migrationBuilder.RenameColumn(
                name: "RequestedBy",
                table: "AdminRequests",
                newName: "AdminId");
        }
    }
}
