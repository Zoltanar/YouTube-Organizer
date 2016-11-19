using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace YoutubeOrganizer.Migrations
{
    public partial class usergroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserGroup",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    ChannelId = table.Column<string>(nullable: false),
                    GroupingTemplate = table.Column<string>(nullable: false),
                    GroupName = table.Column<string>(nullable: true)
                });
            migrationBuilder.Sql(
                "     CONSTRAINT [PK_UserGroup] CHECK (\r\n     ([ChannelId] IS NOT NULL AND [GroupingTemplate] IS NULL AND [VideoId] IS NULL)\r\n  OR ([ChannelId] IS NOT NULL AND [GroupingTemplate] IS NOT NULL AND [VideoId] IS NULL)\r\n  OR ([GroupingTemplate] IS NOT NULL AND [ChannelId] IS NULL AND [VideoId] IS NULL) \r\n  OR ([VideoId] IS NOT NULL AND [ChannelId] IS NULL AND [GroupingTemplate] IS NULL)))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserGroup");
        }
    }
}
