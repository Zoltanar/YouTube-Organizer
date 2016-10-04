using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace YoutubeOrganizer.Migrations
{
    public partial class keychange2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ChannelItem",
                table: "ChannelItem");

            migrationBuilder.DropColumn(
                name: "DatabaseId",
                table: "ChannelItem");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ChannelItem",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChannelItem",
                table: "ChannelItem",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ChannelItem",
                table: "ChannelItem");

            migrationBuilder.AddColumn<int>(
                name: "DatabaseId",
                table: "ChannelItem",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ChannelItem",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChannelItem",
                table: "ChannelItem",
                column: "DatabaseId");
        }
    }
}
