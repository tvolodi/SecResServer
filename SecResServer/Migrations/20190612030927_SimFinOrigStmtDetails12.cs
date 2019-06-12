using Microsoft.EntityFrameworkCore.Migrations;

namespace SecResServer.Migrations
{
    public partial class SimFinOrigStmtDetails12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SimFinOriginalStmtId",
                table: "SimFinOrigStmtDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SimFinOrigStmtDetails_SimFinOriginalStmtId",
                table: "SimFinOrigStmtDetails",
                column: "SimFinOriginalStmtId");

            migrationBuilder.AddForeignKey(
                name: "FK_SimFinOrigStmtDetails_SimFinOriginalStmts_SimFinOriginalStm~",
                table: "SimFinOrigStmtDetails",
                column: "SimFinOriginalStmtId",
                principalTable: "SimFinOriginalStmts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SimFinOrigStmtDetails_SimFinOriginalStmts_SimFinOriginalStm~",
                table: "SimFinOrigStmtDetails");

            migrationBuilder.DropIndex(
                name: "IX_SimFinOrigStmtDetails_SimFinOriginalStmtId",
                table: "SimFinOrigStmtDetails");

            migrationBuilder.DropColumn(
                name: "SimFinOriginalStmtId",
                table: "SimFinOrigStmtDetails");
        }
    }
}
