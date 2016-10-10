using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace YoutubeOrganizer.Migrations
{
    public partial class renameChannelTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "ChannelItem");

            migrationBuilder.AddColumn<string>(
                name: "ChannelTitle",
                table: "ChannelItem",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelTitle",
                table: "ChannelItem");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ChannelItem",
                nullable: true);
        }
    }
}
