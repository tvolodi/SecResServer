using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SecResServer.Migrations
{
    public partial class SimFinStmtDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SimFinStmtDetailTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimFinStmtDetailTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SimFinStmtDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    SimFinStmtRegistryId = table.Column<int>(nullable: false),
                    SimFinStmtDetailTypeId = table.Column<int>(nullable: false),
                    TId = table.Column<int>(nullable: false),
                    ParentTId = table.Column<int>(nullable: false),
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SimFinStmtDetails");

            migrationBuilder.DropTable(
                name: "SimFinStmtDetailTypes");
        }
    }
}
