using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SecResServer.Migrations
{
    public partial class SimFinOrigStmtUpdateDT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStmtLoaded",
                table: "SimFinStmtRegistries");

            migrationBuilder.AddColumn<DateTime>(
                name: "OrigStmtLoadDT",
                table: "SimFinStmtRegistries",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StdStmtLoadDT",
                table: "SimFinStmtRegistries",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrigStmtLoadDT",
                table: "SimFinStmtRegistries");

            migrationBuilder.DropColumn(
                name: "StdStmtLoadDT",
                table: "SimFinStmtRegistries");

            migrationBuilder.AddColumn<bool>(
                name: "IsStmtLoaded",
                table: "SimFinStmtRegistries",
                nullable: false,
                defaultValue: false);
        }
    }
}
