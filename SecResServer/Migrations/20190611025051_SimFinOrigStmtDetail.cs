using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SecResServer.Migrations
{
    public partial class SimFinOrigStmtDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LineItemId",
                table: "SimFinOriginalStmts");

            migrationBuilder.CreateTable(
                name: "StmtDetailNames",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StmtDetailNames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SimFinOrigStmtDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    LineItemId = table.Column<int>(nullable: false),
                    StmtDetailNameId = table.Column<int>(nullable: false),
                    Value = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimFinOrigStmtDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimFinOrigStmtDetails_StmtDetailNames_StmtDetailNameId",
                        column: x => x.StmtDetailNameId,
                        principalTable: "StmtDetailNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SimFinOrigStmtDetails_StmtDetailNameId",
                table: "SimFinOrigStmtDetails",
                column: "StmtDetailNameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SimFinOrigStmtDetails");

            migrationBuilder.DropTable(
                name: "StmtDetailNames");

            migrationBuilder.AddColumn<int>(
                name: "LineItemId",
                table: "SimFinOriginalStmts",
                nullable: false,
                defaultValue: 0);
        }
    }
}
