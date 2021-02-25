using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LoginBase.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionSuas",
                columns: table => new
                {
                    ConfSuaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfSuaNombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfSuaEstatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionSuas", x => x.ConfSuaId);
                });

            migrationBuilder.CreateTable(
                name: "ExcelColumna",
                columns: table => new
                {
                    ExcelColumnaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExcelColumnaNombre = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcelColumna", x => x.ExcelColumnaId);
                });

            migrationBuilder.CreateTable(
                name: "Parametros",
                columns: table => new
                {
                    ParametroId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParametroNombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ParametroClave = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    ParametroDescripcion = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: true),
                    ParametroValorInicial = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ParametroValorFinal = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ParametroEstatusDelete = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parametros", x => x.ParametroId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RolNombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RolEstatus = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RolId);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionSuaNiveles",
                columns: table => new
                {
                    ConfSuaNId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfSuaNNombre = table.Column<int>(type: "int", nullable: false),
                    ConfSuaId = table.Column<int>(type: "int", nullable: false),
                    ConfiguracionSuaConfSuaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionSuaNiveles", x => x.ConfSuaNId);
                    table.ForeignKey(
                        name: "FK_ConfiguracionSuaNiveles_ConfiguracionSuas_ConfiguracionSuaConfSuaId",
                        column: x => x.ConfiguracionSuaConfSuaId,
                        principalTable: "ConfiguracionSuas",
                        principalColumn: "ConfSuaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioNombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioApellidoP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioApellidoM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioNumConfirmacion = table.Column<int>(type: "int", nullable: true),
                    UsuarioFechaLimite = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioEstatusSesion = table.Column<bool>(type: "bit", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UsuarioToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RolId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioId);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "RolId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SuaExcels",
                columns: table => new
                {
                    SuaExcelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoPeriodoId = table.Column<int>(type: "int", nullable: false),
                    ExcelColumnaId = table.Column<int>(type: "int", nullable: false),
                    ConfSuaNId = table.Column<int>(type: "int", nullable: false),
                    ConfiguracionSuaNivelConfSuaNId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuaExcels", x => x.SuaExcelId);
                    table.ForeignKey(
                        name: "FK_SuaExcels_ConfiguracionSuaNiveles_ConfiguracionSuaNivelConfSuaNId",
                        column: x => x.ConfiguracionSuaNivelConfSuaNId,
                        principalTable: "ConfiguracionSuaNiveles",
                        principalColumn: "ConfSuaNId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SuaExcels_ExcelColumna_ExcelColumnaId",
                        column: x => x.ExcelColumnaId,
                        principalTable: "ExcelColumna",
                        principalColumn: "ExcelColumnaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmpleadoColumna",
                columns: table => new
                {
                    EmpleadoColumnaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoColumnaMes = table.Column<int>(type: "int", nullable: false),
                    EmpleadoColumnaAnio = table.Column<int>(type: "int", nullable: false),
                    SuaExcelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadoColumna", x => x.EmpleadoColumnaId);
                    table.ForeignKey(
                        name: "FK_EmpleadoColumna_SuaExcels_SuaExcelId",
                        column: x => x.SuaExcelId,
                        principalTable: "SuaExcels",
                        principalColumn: "SuaExcelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmpleadoColumnaV",
                columns: table => new
                {
                    EmpleadoColumnaVId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoColumnaValor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmpleadoColumnaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadoColumnaV", x => x.EmpleadoColumnaVId);
                    table.ForeignKey(
                        name: "FK_EmpleadoColumnaV_EmpleadoColumna_EmpleadoColumnaId",
                        column: x => x.EmpleadoColumnaId,
                        principalTable: "EmpleadoColumna",
                        principalColumn: "EmpleadoColumnaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionSuaNiveles_ConfiguracionSuaConfSuaId",
                table: "ConfiguracionSuaNiveles",
                column: "ConfiguracionSuaConfSuaId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoColumna_SuaExcelId",
                table: "EmpleadoColumna",
                column: "SuaExcelId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoColumnaV_EmpleadoColumnaId",
                table: "EmpleadoColumnaV",
                column: "EmpleadoColumnaId");

            migrationBuilder.CreateIndex(
                name: "IX_SuaExcels_ConfiguracionSuaNivelConfSuaNId",
                table: "SuaExcels",
                column: "ConfiguracionSuaNivelConfSuaNId");

            migrationBuilder.CreateIndex(
                name: "IX_SuaExcels_ExcelColumnaId",
                table: "SuaExcels",
                column: "ExcelColumnaId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolId",
                table: "Usuarios",
                column: "RolId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpleadoColumnaV");

            migrationBuilder.DropTable(
                name: "Parametros");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "EmpleadoColumna");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "SuaExcels");

            migrationBuilder.DropTable(
                name: "ConfiguracionSuaNiveles");

            migrationBuilder.DropTable(
                name: "ExcelColumna");

            migrationBuilder.DropTable(
                name: "ConfiguracionSuas");
        }
    }
}
