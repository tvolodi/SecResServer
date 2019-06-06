using Microsoft.EntityFrameworkCore.Migrations;

namespace SecResServer.Migrations
{
    public partial class SimFinStmtRegUpper : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_simFinStmtRegistries_PeriodTypes_PeriodTypeId",
                table: "simFinStmtRegistries");

            migrationBuilder.DropForeignKey(
                name: "FK_simFinStmtRegistries_SimFinEntities_SimFinEntityId",
                table: "simFinStmtRegistries");

            migrationBuilder.DropForeignKey(
                name: "FK_simFinStmtRegistries_StmtTypes_StmtTypeId",
                table: "simFinStmtRegistries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_simFinStmtRegistries",
                table: "simFinStmtRegistries");

            migrationBuilder.RenameTable(
                name: "simFinStmtRegistries",
                newName: "SimFinStmtRegistries");

            migrationBuilder.RenameIndex(
                name: "IX_simFinStmtRegistries_StmtTypeId",
                table: "SimFinStmtRegistries",
                newName: "IX_SimFinStmtRegistries_StmtTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_simFinStmtRegistries_SimFinEntityId",
                table: "SimFinStmtRegistries",
                newName: "IX_SimFinStmtRegistries_SimFinEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_simFinStmtRegistries_PeriodTypeId",
                table: "SimFinStmtRegistries",
                newName: "IX_SimFinStmtRegistries_PeriodTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SimFinStmtRegistries",
                table: "SimFinStmtRegistries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SimFinStmtRegistries_PeriodTypes_PeriodTypeId",
                table: "SimFinStmtRegistries",
                column: "PeriodTypeId",
                principalTable: "PeriodTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SimFinStmtRegistries_SimFinEntities_SimFinEntityId",
                table: "SimFinStmtRegistries",
                column: "SimFinEntityId",
                principalTable: "SimFinEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SimFinStmtRegistries_StmtTypes_StmtTypeId",
                table: "SimFinStmtRegistries",
                column: "StmtTypeId",
                principalTable: "StmtTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SimFinStmtRegistries_PeriodTypes_PeriodTypeId",
                table: "SimFinStmtRegistries");

            migrationBuilder.DropForeignKey(
                name: "FK_SimFinStmtRegistries_SimFinEntities_SimFinEntityId",
                table: "SimFinStmtRegistries");

            migrationBuilder.DropForeignKey(
                name: "FK_SimFinStmtRegistries_StmtTypes_StmtTypeId",
                table: "SimFinStmtRegistries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SimFinStmtRegistries",
                table: "SimFinStmtRegistries");

            migrationBuilder.RenameTable(
                name: "SimFinStmtRegistries",
                newName: "simFinStmtRegistries");

            migrationBuilder.RenameIndex(
                name: "IX_SimFinStmtRegistries_StmtTypeId",
                table: "simFinStmtRegistries",
                newName: "IX_simFinStmtRegistries_StmtTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_SimFinStmtRegistries_SimFinEntityId",
                table: "simFinStmtRegistries",
                newName: "IX_simFinStmtRegistries_SimFinEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_SimFinStmtRegistries_PeriodTypeId",
                table: "simFinStmtRegistries",
                newName: "IX_simFinStmtRegistries_PeriodTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_simFinStmtRegistries",
                table: "simFinStmtRegistries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_simFinStmtRegistries_PeriodTypes_PeriodTypeId",
                table: "simFinStmtRegistries",
                column: "PeriodTypeId",
                principalTable: "PeriodTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_simFinStmtRegistries_SimFinEntities_SimFinEntityId",
                table: "simFinStmtRegistries",
                column: "SimFinEntityId",
                principalTable: "SimFinEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_simFinStmtRegistries_StmtTypes_StmtTypeId",
                table: "simFinStmtRegistries",
                column: "StmtTypeId",
                principalTable: "StmtTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
