using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace YoutubeOrganizer.Migrations
{
    public partial class keychange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoItem",
                table: "VideoItem");

            migrationBuilder.DropColumn(
                name: "DatabaseId",
                table: "VideoItem");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "VideoItem",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoItem",
                table: "VideoItem",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoItem",
                table: "VideoItem");

            migrationBuilder.AddColumn<int>(
                name: "DatabaseId",
                table: "VideoItem",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "VideoItem",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoItem",
                table: "VideoItem",
                column: "DatabaseId");
        }
    }
}
