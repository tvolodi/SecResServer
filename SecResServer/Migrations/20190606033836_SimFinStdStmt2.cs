using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SecResServer.Migrations
{
    public partial class SimFinStdStmt2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SimFinStmtDetails");

            migrationBuilder.RenameColumn(
                name: "IsStmtDetailsLoaded",
                table: "SimFinStmtRegistries",
                newName: "IsStmtLoaded");

            migrationBuilder.AddColumn<int>(
                name: "SimFinStmtIndustryTemplateId",
                table: "SimFinStmtRegistries",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SimFinStdStmts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    GetFinStmtRegistryId = table.Column<int>(nullable: false),
                    SimFinStmtIndustryTemplateId = table.Column<int>(nullable: false),
                    PeriodEndDate = table.Column<DateTime>(nullable: false),
                    IsStmtDetailsLoaded = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimFinStdStmts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimFinStdStmts_SimFinStmtRegistries_GetFinStmtRegistryId",
                        column: x => x.GetFinStmtRegistryId,
                        principalTable: "SimFinStmtRegistries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "SimFinStdStmtDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    SimFinStdStmtId = table.Column<int>(nullable: false),
                    SimFinStmtDetailTypeId = table.Column<int>(nullable: false),
                    TId = table.Column<int>(nullable: false),
                    ParentTId = table.Column<int>(nullable: false),
                    Value = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimFinStdStmtDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimFinStdStmtDetails_SimFinStdStmts_SimFinStdStmtId",
                        column: x => x.SimFinStdStmtId,
                        principalTable: "SimFinStdStmts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SimFinStdStmtDetails_SimFinStmtDetailTypes_SimFinStmtDetail~",
                        column: x => x.SimFinStmtDetailTypeId,
                        principalTable: "SimFinStmtDetailTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SimFinStmtRegistries_SimFinStmtIndustryTemplateId",
                table: "SimFinStmtRegistries",
                column: "SimFinStmtIndustryTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SimFinStdStmtDetails_SimFinStdStmtId",
                table: "SimFinStdStmtDetails",
                column: "SimFinStdStmtId");

            migrationBuilder.CreateIndex(
                name: "IX_SimFinStdStmtDetails_SimFinStmtDetailTypeId",
                table: "SimFinStdStmtDetails",
                column: "SimFinStmtDetailTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SimFinStdStmts_GetFinStmtRegistryId",
                table: "SimFinStdStmts",
                column: "GetFinStmtRegistryId");

            migrationBuilder.AddForeignKey(
                name: "FK_SimFinStmtRegistries_SimFinStmtIndustryTemplate_SimFinStmtI~",
                table: "SimFinStmtRegistries",
                column: "SimFinStmtIndustryTemplateId",
                principalTable: "SimFinStmtIndustryTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SimFinStmtRegistries_SimFinStmtIndustryTemplate_SimFinStmtI~",
                table: "SimFinStmtRegistries");

            migrationBuilder.DropTable(
                name: "SimFinStdStmtDetails");

            migrationBuilder.DropTable(
                name: "SimFinStmtIndustryTemplate");

            migrationBuilder.DropTable(
                name: "SimFinStdStmts");

            migrationBuilder.DropIndex(
                name: "IX_SimFinStmtRegistries_SimFinStmtIndustryTemplateId",
                table: "SimFinStmtRegistries");

            migrationBuilder.DropColumn(
                name: "SimFinStmtIndustryTemplateId",
                table: "SimFinStmtRegistries");

            migrationBuilder.RenameColumn(
                name: "IsStmtLoaded",
                table: "SimFinStmtRegistries",
                newName: "IsStmtDetailsLoaded");

            migrationBuilder.CreateTable(
                name: "SimFinStmtDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ParentTId = table.Column<int>(nullable: false),
                    SimFinStmtDetailTypeId = table.Column<int>(nullable: false),
                    SimFinStmtRegistryId = table.Column<int>(nullable: false),
                    TId = table.Column<int>(nullable: false),
                    Value = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimFinStmtDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimFinStmtDetails_SimFinStmtDetailTypes_SimFinStmtDetailTyp~",
                        column: x => x.SimFinStmtDetailTypeId,
                        principalTable: "SimFinStmtDetailTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SimFinStmtDetails_SimFinStmtRegistries_SimFinStmtRegistryId",
                        column: x => x.SimFinStmtRegistryId,
                        principalTable: "SimFinStmtRegistries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SimFinStmtDetails_SimFinStmtDetailTypeId",
                table: "SimFinStmtDetails",
                column: "SimFinStmtDetailTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SimFinStmtDetails_SimFinStmtRegistryId",
                table: "SimFinStmtDetails",
                column: "SimFinStmtRegistryId");
        }
    }
}
