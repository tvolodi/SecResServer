using Microsoft.EntityFrameworkCore.Migrations;

namespace SecResServer.Migrations
{
    public partial class SimFinIndustry2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SimFinSectors",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Name",
                table: "SimFinSectors",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
