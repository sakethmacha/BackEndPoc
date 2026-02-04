using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ScreenNameScreenId",
                table: "AdminRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TheatreNameTheatreId",
                table: "AdminRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AdminRequests_ScreenNameScreenId",
                table: "AdminRequests",
                column: "ScreenNameScreenId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminRequests_TheatreNameTheatreId",
                table: "AdminRequests",
                column: "TheatreNameTheatreId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminRequests_Screens_ScreenNameScreenId",
                table: "AdminRequests",
                column: "ScreenNameScreenId",
                principalTable: "Screens",
                principalColumn: "ScreenId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdminRequests_Theatres_TheatreNameTheatreId",
                table: "AdminRequests",
                column: "TheatreNameTheatreId",
                principalTable: "Theatres",
                principalColumn: "TheatreId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminRequests_Screens_ScreenNameScreenId",
                table: "AdminRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_AdminRequests_Theatres_TheatreNameTheatreId",
                table: "AdminRequests");

            migrationBuilder.DropIndex(
                name: "IX_AdminRequests_ScreenNameScreenId",
                table: "AdminRequests");

            migrationBuilder.DropIndex(
                name: "IX_AdminRequests_TheatreNameTheatreId",
                table: "AdminRequests");

            migrationBuilder.DropColumn(
                name: "ScreenNameScreenId",
                table: "AdminRequests");

            migrationBuilder.DropColumn(
                name: "TheatreNameTheatreId",
                table: "AdminRequests");
        }
    }
}
