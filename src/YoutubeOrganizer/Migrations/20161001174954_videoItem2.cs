using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace YoutubeOrganizer.Migrations
{
    public partial class videoItem2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VideoItem",
                columns: table => new
                {
                    DatabaseId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChannelId = table.Column<string>(nullable: true),
                    Duration = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: true),
                    ThumbnailHeight = table.Column<long>(nullable: true),
                    ThumbnailUrl = table.Column<string>(nullable: true),
                    ThumbnailWidth = table.Column<long>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoItem", x => x.DatabaseId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoItem");
        }
    }
}
