using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SecResServer.Migrations
{
    public partial class SimFinoOrigStmt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SimFinStmtRegistries_SimFinStmtIndustryTemplate_SimFinStmtI~",
                table: "SimFinStmtRegistries");

            migrationBuilder.DropTable(
                name: "SimFinStmtIndustryTemplate");

            migrationBuilder.DropIndex(
                name: "IX_SimFinStmtRegistries_SimFinStmtIndustryTemplateId",
                table: "SimFinStmtRegistries");

            migrationBuilder.DropColumn(
                name: "SimFinStmtIndustryTemplateId",
                table: "SimFinStmtRegistries");

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CharCode = table.Column<string>(nullable: true),
                    CountryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Currencies_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SimFinOriginalStmts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    SimFinStmtRegistryId = table.Column<int>(nullable: false),
                    PeriodEndDate = table.Column<DateTime>(nullable: false),
                    FirstPublishedDate = table.Column<DateTime>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    PeriodTypeId = table.Column<int>(nullable: false),
                    FYear = table.Column<int>(nullable: false),
                    IsStmtDetailsLoaded = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimFinOriginalStmts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimFinOriginalStmts_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SimFinOriginalStmts_PeriodTypes_PeriodTypeId",
                        column: x => x.PeriodTypeId,
                        principalTable: "PeriodTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SimFinOriginalStmts_SimFinStmtRegistries_SimFinStmtRegistry~",
                        column: x => x.SimFinStmtRegistryId,
                        principalTable: "SimFinStmtRegistries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CharCode",
                table: "Currencies",
                column: "CharCode");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CountryId",
                table: "Currencies",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Id",
                table: "Currencies",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SimFinOriginalStmts_CurrencyId",
                table: "SimFinOriginalStmts",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_SimFinOriginalStmts_PeriodTypeId",
                table: "SimFinOriginalStmts",
                column: "PeriodTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SimFinOriginalStmts_SimFinStmtRegistryId",
                table: "SimFinOriginalStmts",
                column: "SimFinStmtRegistryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SimFinOriginalStmts");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.AddColumn<int>(
                name: "SimFinStmtIndustryTemplateId",
                table: "SimFinStmtRegistries",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SimFinStmtIndustryTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimFinStmtIndustryTemplate", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SimFinStmtRegistries_SimFinStmtIndustryTemplateId",
                table: "SimFinStmtRegistries",
                column: "SimFinStmtIndustryTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_SimFinStmtRegistries_SimFinStmtIndustryTemplate_SimFinStmtI~",
                table: "SimFinStmtRegistries",
                column: "SimFinStmtIndustryTemplateId",
                principalTable: "SimFinStmtIndustryTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
