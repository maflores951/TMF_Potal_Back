using Microsoft.EntityFrameworkCore.Migrations;

namespace LoginBase.Migrations
{
    public partial class ModificarExcelTipos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcelColumnaId",
                table: "ExcelTipos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExcelColumnaId",
                table: "ExcelTipos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
