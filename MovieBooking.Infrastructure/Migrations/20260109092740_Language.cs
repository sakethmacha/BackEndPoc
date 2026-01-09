using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Language : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>( 
                name: "MovieId1",
                table: "ShowTimes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ScreenId1",
                table: "ShowTimes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TheatreId1",
                table: "ShowTimes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TheatreId1",
                table: "Screens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    LanguageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.LanguageId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShowTimes_LanguageId",
                table: "ShowTimes",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowTimes_MovieId1",
                table: "ShowTimes",
                column: "MovieId1");

            migrationBuilder.CreateIndex(
                name: "IX_ShowTimes_ScreenId1",
                table: "ShowTimes",
                column: "ScreenId1");

            migrationBuilder.CreateIndex(
                name: "IX_ShowTimes_TheatreId1",
                table: "ShowTimes",
                column: "TheatreId1");

            migrationBuilder.CreateIndex(
                name: "IX_Screens_TheatreId1",
                table: "Screens",
                column: "TheatreId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Screens_Theatres_TheatreId1",
                table: "Screens",
                column: "TheatreId1",
                principalTable: "Theatres",
                principalColumn: "TheatreId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShowTimes_Languages_LanguageId",
                table: "ShowTimes",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "LanguageId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShowTimes_Movies_MovieId1",
                table: "ShowTimes",
                column: "MovieId1",
                principalTable: "Movies",
                principalColumn: "MovieId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShowTimes_Screens_ScreenId1",
                table: "ShowTimes",
                column: "ScreenId1",
                principalTable: "Screens",
                principalColumn: "ScreenId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShowTimes_Theatres_TheatreId1",
                table: "ShowTimes",
                column: "TheatreId1",
                principalTable: "Theatres",
                principalColumn: "TheatreId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Screens_Theatres_TheatreId1",
                table: "Screens");

            migrationBuilder.DropForeignKey(
                name: "FK_ShowTimes_Languages_LanguageId",
                table: "ShowTimes");

            migrationBuilder.DropForeignKey(
                name: "FK_ShowTimes_Movies_MovieId1",
                table: "ShowTimes");

            migrationBuilder.DropForeignKey(
                name: "FK_ShowTimes_Screens_ScreenId1",
                table: "ShowTimes");

            migrationBuilder.DropForeignKey(
                name: "FK_ShowTimes_Theatres_TheatreId1",
                table: "ShowTimes");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_ShowTimes_LanguageId",
                table: "ShowTimes");

            migrationBuilder.DropIndex(
                name: "IX_ShowTimes_MovieId1",
                table: "ShowTimes");

            migrationBuilder.DropIndex(
                name: "IX_ShowTimes_ScreenId1",
                table: "ShowTimes");

            migrationBuilder.DropIndex(
                name: "IX_ShowTimes_TheatreId1",
                table: "ShowTimes");

            migrationBuilder.DropIndex(
                name: "IX_Screens_TheatreId1",
                table: "Screens");

            migrationBuilder.DropColumn(
                name: "MovieId1",
                table: "ShowTimes");

            migrationBuilder.DropColumn(
                name: "ScreenId1",
                table: "ShowTimes");

            migrationBuilder.DropColumn(
                name: "TheatreId1",
                table: "ShowTimes");

            migrationBuilder.DropColumn(
                name: "TheatreId1",
                table: "Screens");
        }
    }
}
