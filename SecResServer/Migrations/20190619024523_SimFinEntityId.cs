using Microsoft.EntityFrameworkCore.Migrations;

namespace SecResServer.Migrations
{
    public partial class SimFinEntityId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStmtRegistryLoaded",
                table: "SimFinEntities",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStmtRegistryLoaded",
                table: "SimFinEntities");
        }
    }
}
