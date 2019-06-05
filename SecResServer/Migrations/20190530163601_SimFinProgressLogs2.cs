using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SecResServer.Migrations
{
    public partial class SimFinProgressLogs2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "simFinStmtRegistries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    SimFinEntityId = table.Column<int>(nullable: false),
                    StmtTypeId = table.Column<int>(nullable: false),
                    FYear = table.Column<int>(nullable: false),
                    PeriodTypeId = table.Column<int>(nullable: false),
                    LoadDateTime = table.Column<DateTime>(nullable: false),
                    IsCalculated = table.Column<bool>(nullable: false),
                    IsStmtDetailsLoaded = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_simFinStmtRegistries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_simFinStmtRegistries_PeriodTypes_PeriodTypeId",
                        column: x => x.PeriodTypeId,
                        principalTable: "PeriodTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_simFinStmtRegistries_SimFinEntities_SimFinEntityId",
                        column: x => x.SimFinEntityId,
                        principalTable: "SimFinEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_simFinStmtRegistries_StmtTypes_StmtTypeId",
                        column: x => x.StmtTypeId,
                        principalTable: "StmtTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_simFinStmtRegistries_PeriodTypeId",
                table: "simFinStmtRegistries",
                column: "PeriodTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_simFinStmtRegistries_SimFinEntityId",
                table: "simFinStmtRegistries",
                column: "SimFinEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_simFinStmtRegistries_StmtTypeId",
                table: "simFinStmtRegistries",
                column: "StmtTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "simFinStmtRegistries");
        }
    }
}
