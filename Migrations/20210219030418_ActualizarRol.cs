using Microsoft.EntityFrameworkCore.Migrations;

namespace LoginBase.Migrations
{
    public partial class ActualizarRol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RolDelete",
                table: "Roles",
                newName: "RolEstatus");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RolEstatus",
                table: "Roles",
                newName: "RolDelete");
        }
    }
}
