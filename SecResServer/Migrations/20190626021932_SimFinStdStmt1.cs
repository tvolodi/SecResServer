using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SecResServer.Migrations
{
    public partial class SimFinStdStmt1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SimFinStdStmtDetails_SimFinStmtDetailTypes_SimFinStmtDetail~",
                table: "SimFinStdStmtDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SimFinStdStmts_SimFinStmtRegistries_GetFinStmtRegistryId",
                table: "SimFinStdStmts");

            migrationBuilder.DropIndex(
                name: "IX_SimFinStdStmtDetails_SimFinStmtDetailTypeId",
                table: "SimFinStdStmtDetails");

            migrationBuilder.RenameColumn(
                name: "GetFinStmtRegistryId",
                table: "SimFinStdStmts",
                newName: "SimFinStmtRegistryId");

            migrationBuilder.RenameIndex(
                name: "IX_SimFinStdStmts_GetFinStmtRegistryId",
                table: "SimFinStdStmts",
                newName: "IX_SimFinStdStmts_SimFinStmtRegistryId");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "SimFinStdStmtDetails",
                newName: "ValueChosen");

            migrationBuilder.RenameColumn(
                name: "SimFinStmtDetailTypeId",
                table: "SimFinStdStmtDetails",
                newName: "UId");

            migrationBuilder.AddColumn<int>(
                name: "FYear",
                table: "SimFinStdStmts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PeriodTypeId",
                table: "SimFinStdStmts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DisplayLevel",
                table: "SimFinStdStmtDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StmtDetailNameId",
                table: "SimFinStdStmtDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "ValueAssigned",
                table: "SimFinStdStmtDetails",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ValueCalculated",
                table: "SimFinStdStmtDetails",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "SimFinStmtIndustryTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimFinStmtIndustryTemplates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SimFinStdStmts_PeriodTypeId",
                table: "SimFinStdStmts",
                column: "PeriodTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SimFinStdStmtDetails_StmtDetailNameId",
                table: "SimFinStdStmtDetails",
                column: "StmtDetailNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_SimFinStdStmtDetails_StmtDetailNames_StmtDetailNameId",
                table: "SimFinStdStmtDetails",
                column: "StmtDetailNameId",
                principalTable: "StmtDetailNames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SimFinStdStmts_PeriodTypes_PeriodTypeId",
                table: "SimFinStdStmts",
                column: "PeriodTypeId",
                principalTable: "PeriodTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SimFinStdStmts_SimFinStmtRegistries_SimFinStmtRegistryId",
                table: "SimFinStdStmts",
                column: "SimFinStmtRegistryId",
                principalTable: "SimFinStmtRegistries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SimFinStdStmtDetails_StmtDetailNames_StmtDetailNameId",
                table: "SimFinStdStmtDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SimFinStdStmts_PeriodTypes_PeriodTypeId",
                table: "SimFinStdStmts");

            migrationBuilder.DropForeignKey(
                name: "FK_SimFinStdStmts_SimFinStmtRegistries_SimFinStmtRegistryId",
                table: "SimFinStdStmts");

            migrationBuilder.DropTable(
                name: "SimFinStmtIndustryTemplates");

            migrationBuilder.DropIndex(
                name: "IX_SimFinStdStmts_PeriodTypeId",
                table: "SimFinStdStmts");

            migrationBuilder.DropIndex(
                name: "IX_SimFinStdStmtDetails_StmtDetailNameId",
                table: "SimFinStdStmtDetails");

            migrationBuilder.DropColumn(
                name: "FYear",
                table: "SimFinStdStmts");

            migrationBuilder.DropColumn(
                name: "PeriodTypeId",
                table: "SimFinStdStmts");

            migrationBuilder.DropColumn(
                name: "DisplayLevel",
                table: "SimFinStdStmtDetails");

            migrationBuilder.DropColumn(
                name: "StmtDetailNameId",
                table: "SimFinStdStmtDetails");

            migrationBuilder.DropColumn(
                name: "ValueAssigned",
                table: "SimFinStdStmtDetails");

            migrationBuilder.DropColumn(
                name: "ValueCalculated",
                table: "SimFinStdStmtDetails");

            migrationBuilder.RenameColumn(
                name: "SimFinStmtRegistryId",
                table: "SimFinStdStmts",
                newName: "GetFinStmtRegistryId");

            migrationBuilder.RenameIndex(
                name: "IX_SimFinStdStmts_SimFinStmtRegistryId",
                table: "SimFinStdStmts",
                newName: "IX_SimFinStdStmts_GetFinStmtRegistryId");

            migrationBuilder.RenameColumn(
                name: "ValueChosen",
                table: "SimFinStdStmtDetails",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "UId",
                table: "SimFinStdStmtDetails",
                newName: "SimFinStmtDetailTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SimFinStdStmtDetails_SimFinStmtDetailTypeId",
                table: "SimFinStdStmtDetails",
                column: "SimFinStmtDetailTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SimFinStdStmtDetails_SimFinStmtDetailTypes_SimFinStmtDetail~",
                table: "SimFinStdStmtDetails",
                column: "SimFinStmtDetailTypeId",
                principalTable: "SimFinStmtDetailTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SimFinStdStmts_SimFinStmtRegistries_GetFinStmtRegistryId",
                table: "SimFinStdStmts",
                column: "GetFinStmtRegistryId",
                principalTable: "SimFinStmtRegistries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
