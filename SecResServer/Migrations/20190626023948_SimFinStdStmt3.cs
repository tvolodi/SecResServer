using Microsoft.EntityFrameworkCore.Migrations;

namespace SecResServer.Migrations
{
    public partial class SimFinStdStmt3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SimFinStdStmts_SimFinStmtIndustryTemplateId",
                table: "SimFinStdStmts",
                column: "SimFinStmtIndustryTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_SimFinStdStmts_SimFinStmtIndustryTemplates_SimFinStmtIndust~",
                table: "SimFinStdStmts",
                column: "SimFinStmtIndustryTemplateId",
                principalTable: "SimFinStmtIndustryTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SimFinStdStmts_SimFinStmtIndustryTemplates_SimFinStmtIndust~",
                table: "SimFinStdStmts");

            migrationBuilder.DropIndex(
                name: "IX_SimFinStdStmts_SimFinStmtIndustryTemplateId",
                table: "SimFinStdStmts");
        }
    }
}
