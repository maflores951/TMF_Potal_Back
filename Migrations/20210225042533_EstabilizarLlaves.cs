using Microsoft.EntityFrameworkCore.Migrations;

namespace LoginBase.Migrations
{
    public partial class EstabilizarLlaves : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfiguracionSuaNiveles_ConfiguracionSuas_ConfiguracionSuaConfSuaId",
                table: "ConfiguracionSuaNiveles");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpleadoColumna_SuaExcels_SuaExcelId",
                table: "EmpleadoColumna");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpleadoColumnaV_EmpleadoColumna_EmpleadoColumnaId",
                table: "EmpleadoColumnaV");

            migrationBuilder.DropForeignKey(
                name: "FK_SuaExcels_ConfiguracionSuaNiveles_ConfiguracionSuaNivelConfSuaNId",
                table: "SuaExcels");

            migrationBuilder.DropForeignKey(
                name: "FK_SuaExcels_ExcelColumna_ExcelColumnaId",
                table: "SuaExcels");

            migrationBuilder.DropIndex(
                name: "IX_SuaExcels_ConfiguracionSuaNivelConfSuaNId",
                table: "SuaExcels");

            migrationBuilder.DropIndex(
                name: "IX_ConfiguracionSuaNiveles_ConfiguracionSuaConfSuaId",
                table: "ConfiguracionSuaNiveles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExcelColumna",
                table: "ExcelColumna");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpleadoColumna",
                table: "EmpleadoColumna");

            migrationBuilder.DropColumn(
                name: "ConfiguracionSuaNivelConfSuaNId",
                table: "SuaExcels");

            migrationBuilder.DropColumn(
                name: "ConfiguracionSuaConfSuaId",
                table: "ConfiguracionSuaNiveles");

            migrationBuilder.RenameTable(
                name: "ExcelColumna",
                newName: "ExcelColumnas");

            migrationBuilder.RenameTable(
                name: "EmpleadoColumna",
                newName: "EmpleadoColumnas");

            migrationBuilder.RenameColumn(
                name: "ConfSuaNId",
                table: "SuaExcels",
                newName: "ConfiguracionSuaNivelId");

            migrationBuilder.RenameColumn(
                name: "ConfSuaId",
                table: "ConfiguracionSuas",
                newName: "ConfiguracionSuaId");

            migrationBuilder.RenameColumn(
                name: "ConfSuaId",
                table: "ConfiguracionSuaNiveles",
                newName: "ConfiguracionSuaId");

            migrationBuilder.RenameColumn(
                name: "ConfSuaNId",
                table: "ConfiguracionSuaNiveles",
                newName: "ConfiguracionSuaNivelId");

            migrationBuilder.RenameIndex(
                name: "IX_EmpleadoColumna_SuaExcelId",
                table: "EmpleadoColumnas",
                newName: "IX_EmpleadoColumnas_SuaExcelId");

            migrationBuilder.AddColumn<int>(
                name: "ExcelTipoId",
                table: "ExcelColumnas",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExcelColumnas",
                table: "ExcelColumnas",
                column: "ExcelColumnaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpleadoColumnas",
                table: "EmpleadoColumnas",
                column: "EmpleadoColumnaId");

            migrationBuilder.CreateTable(
                name: "ExcelTipos",
                columns: table => new
                {
                    ExcelTipoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExcelNombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcelTipoDescripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcelColumnaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcelTipos", x => x.ExcelTipoId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SuaExcels_ConfiguracionSuaNivelId",
                table: "SuaExcels",
                column: "ConfiguracionSuaNivelId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionSuaNiveles_ConfiguracionSuaId",
                table: "ConfiguracionSuaNiveles",
                column: "ConfiguracionSuaId");

            migrationBuilder.CreateIndex(
                name: "IX_ExcelColumnas_ExcelTipoId",
                table: "ExcelColumnas",
                column: "ExcelTipoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConfiguracionSuaNiveles_ConfiguracionSuas_ConfiguracionSuaId",
                table: "ConfiguracionSuaNiveles",
                column: "ConfiguracionSuaId",
                principalTable: "ConfiguracionSuas",
                principalColumn: "ConfiguracionSuaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpleadoColumnas_SuaExcels_SuaExcelId",
                table: "EmpleadoColumnas",
                column: "SuaExcelId",
                principalTable: "SuaExcels",
                principalColumn: "SuaExcelId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpleadoColumnaV_EmpleadoColumnas_EmpleadoColumnaId",
                table: "EmpleadoColumnaV",
                column: "EmpleadoColumnaId",
                principalTable: "EmpleadoColumnas",
                principalColumn: "EmpleadoColumnaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExcelColumnas_ExcelTipos_ExcelTipoId",
                table: "ExcelColumnas",
                column: "ExcelTipoId",
                principalTable: "ExcelTipos",
                principalColumn: "ExcelTipoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SuaExcels_ConfiguracionSuaNiveles_ConfiguracionSuaNivelId",
                table: "SuaExcels",
                column: "ConfiguracionSuaNivelId",
                principalTable: "ConfiguracionSuaNiveles",
                principalColumn: "ConfiguracionSuaNivelId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SuaExcels_ExcelColumnas_ExcelColumnaId",
                table: "SuaExcels",
                column: "ExcelColumnaId",
                principalTable: "ExcelColumnas",
                principalColumn: "ExcelColumnaId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfiguracionSuaNiveles_ConfiguracionSuas_ConfiguracionSuaId",
                table: "ConfiguracionSuaNiveles");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpleadoColumnas_SuaExcels_SuaExcelId",
                table: "EmpleadoColumnas");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpleadoColumnaV_EmpleadoColumnas_EmpleadoColumnaId",
                table: "EmpleadoColumnaV");

            migrationBuilder.DropForeignKey(
                name: "FK_ExcelColumnas_ExcelTipos_ExcelTipoId",
                table: "ExcelColumnas");

            migrationBuilder.DropForeignKey(
                name: "FK_SuaExcels_ConfiguracionSuaNiveles_ConfiguracionSuaNivelId",
                table: "SuaExcels");

            migrationBuilder.DropForeignKey(
                name: "FK_SuaExcels_ExcelColumnas_ExcelColumnaId",
                table: "SuaExcels");

            migrationBuilder.DropTable(
                name: "ExcelTipos");

            migrationBuilder.DropIndex(
                name: "IX_SuaExcels_ConfiguracionSuaNivelId",
                table: "SuaExcels");

            migrationBuilder.DropIndex(
                name: "IX_ConfiguracionSuaNiveles_ConfiguracionSuaId",
                table: "ConfiguracionSuaNiveles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExcelColumnas",
                table: "ExcelColumnas");

            migrationBuilder.DropIndex(
                name: "IX_ExcelColumnas_ExcelTipoId",
                table: "ExcelColumnas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpleadoColumnas",
                table: "EmpleadoColumnas");

            migrationBuilder.DropColumn(
                name: "ExcelTipoId",
                table: "ExcelColumnas");

            migrationBuilder.RenameTable(
                name: "ExcelColumnas",
                newName: "ExcelColumna");

            migrationBuilder.RenameTable(
                name: "EmpleadoColumnas",
                newName: "EmpleadoColumna");

            migrationBuilder.RenameColumn(
                name: "ConfiguracionSuaNivelId",
                table: "SuaExcels",
                newName: "ConfSuaNId");

            migrationBuilder.RenameColumn(
                name: "ConfiguracionSuaId",
                table: "ConfiguracionSuas",
                newName: "ConfSuaId");

            migrationBuilder.RenameColumn(
                name: "ConfiguracionSuaId",
                table: "ConfiguracionSuaNiveles",
                newName: "ConfSuaId");

            migrationBuilder.RenameColumn(
                name: "ConfiguracionSuaNivelId",
                table: "ConfiguracionSuaNiveles",
                newName: "ConfSuaNId");

            migrationBuilder.RenameIndex(
                name: "IX_EmpleadoColumnas_SuaExcelId",
                table: "EmpleadoColumna",
                newName: "IX_EmpleadoColumna_SuaExcelId");

            migrationBuilder.AddColumn<int>(
                name: "ConfiguracionSuaNivelConfSuaNId",
                table: "SuaExcels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ConfiguracionSuaConfSuaId",
                table: "ConfiguracionSuaNiveles",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExcelColumna",
                table: "ExcelColumna",
                column: "ExcelColumnaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpleadoColumna",
                table: "EmpleadoColumna",
                column: "EmpleadoColumnaId");

            migrationBuilder.CreateIndex(
                name: "IX_SuaExcels_ConfiguracionSuaNivelConfSuaNId",
                table: "SuaExcels",
                column: "ConfiguracionSuaNivelConfSuaNId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionSuaNiveles_ConfiguracionSuaConfSuaId",
                table: "ConfiguracionSuaNiveles",
                column: "ConfiguracionSuaConfSuaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConfiguracionSuaNiveles_ConfiguracionSuas_ConfiguracionSuaConfSuaId",
                table: "ConfiguracionSuaNiveles",
                column: "ConfiguracionSuaConfSuaId",
                principalTable: "ConfiguracionSuas",
                principalColumn: "ConfSuaId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpleadoColumna_SuaExcels_SuaExcelId",
                table: "EmpleadoColumna",
                column: "SuaExcelId",
                principalTable: "SuaExcels",
                principalColumn: "SuaExcelId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpleadoColumnaV_EmpleadoColumna_EmpleadoColumnaId",
                table: "EmpleadoColumnaV",
                column: "EmpleadoColumnaId",
                principalTable: "EmpleadoColumna",
                principalColumn: "EmpleadoColumnaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SuaExcels_ConfiguracionSuaNiveles_ConfiguracionSuaNivelConfSuaNId",
                table: "SuaExcels",
                column: "ConfiguracionSuaNivelConfSuaNId",
                principalTable: "ConfiguracionSuaNiveles",
                principalColumn: "ConfSuaNId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SuaExcels_ExcelColumna_ExcelColumnaId",
                table: "SuaExcels",
                column: "ExcelColumnaId",
                principalTable: "ExcelColumna",
                principalColumn: "ExcelColumnaId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
