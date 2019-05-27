using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SecResServer.Migrations
{
    public partial class EdgarCompanyUpdateDT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EdgarCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Cik = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    EntityId = table.Column<int>(nullable: false),
                    PrimaryExchange = table.Column<string>(nullable: true),
                    MarketOperator = table.Column<string>(nullable: true),
                    Markettier = table.Column<string>(nullable: true),
                    PrimarySymbol = table.Column<string>(nullable: true),
                    SicCode = table.Column<int>(nullable: false),
                    SicDescription = table.Column<string>(nullable: true),
                    LastUpdateDT = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EdgarCompanies", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EdgarCompanies");
        }
    }
}
