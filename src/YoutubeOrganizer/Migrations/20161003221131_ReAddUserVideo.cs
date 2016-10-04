using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace YoutubeOrganizer.Migrations
{
    public partial class ReAddUserVideo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserVideo",
                columns: table => new
                {
                    UserID = table.Column<string>(nullable: false),
                    VideoId = table.Column<string>(nullable: false),
                    Watched = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVideo", x => new { x.UserID, x.VideoId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserVideo");
        }
    }
}
