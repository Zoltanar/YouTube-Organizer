using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace YoutubeOrganizer.Migrations
{
    public partial class keychange3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VideoId",
                table: "UserVideo",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "UserVideo",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "VideoId",
                table: "UserVideo",
                nullable: false);

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "UserVideo",
                nullable: false);
        }
    }
}
