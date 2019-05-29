using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SecResServer.Migrations
{
    public partial class SimFinProgressLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsInitDataLoaded",
                table: "SimFinEntityProgresses",
                newName: "IsInitStmtLoaded");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompanyDataLoaded",
                table: "SimFinEntityProgresses",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SimFinRequestLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    RequestDT = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimFinRequestLogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SimFinRequestLogs");

            migrationBuilder.DropColumn(
                name: "IsCompanyDataLoaded",
                table: "SimFinEntityProgresses");

            migrationBuilder.RenameColumn(
                name: "IsInitStmtLoaded",
                table: "SimFinEntityProgresses",
                newName: "IsInitDataLoaded");
        }
    }
}
