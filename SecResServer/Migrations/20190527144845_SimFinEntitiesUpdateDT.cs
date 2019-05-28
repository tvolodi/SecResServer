using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SecResServer.Migrations
{
    public partial class SimFinEntitiesUpdateDT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_simFinEntities",
                table: "simFinEntities");

            migrationBuilder.RenameTable(
                name: "simFinEntities",
                newName: "SimFinEntities");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDT",
                table: "SimFinEntities",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdateDT",
                table: "SimFinEntities",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDT",
                table: "EdgarCompanies",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SimFinEntities",
                table: "SimFinEntities",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SimFinEntities",
                table: "SimFinEntities");

            migrationBuilder.DropColumn(
                name: "DeleteDT",
                table: "SimFinEntities");

            migrationBuilder.DropColumn(
                name: "LastUpdateDT",
                table: "SimFinEntities");

            migrationBuilder.DropColumn(
                name: "DeleteDT",
                table: "EdgarCompanies");

            migrationBuilder.RenameTable(
                name: "SimFinEntities",
                newName: "simFinEntities");

            migrationBuilder.AddPrimaryKey(
                name: "PK_simFinEntities",
                table: "simFinEntities",
                column: "Id");
        }
    }
}
