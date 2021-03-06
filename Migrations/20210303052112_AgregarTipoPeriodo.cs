using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LoginBase.Migrations
{
    public partial class AgregarTipoPeriodo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionSuas",
                columns: table => new
                {
                    ConfiguracionSuaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfSuaNombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfSuaEstatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionSuas", x => x.ConfiguracionSuaId);
                });

            migrationBuilder.CreateTable(
                name: "ExcelTipos",
                columns: table => new
                {
                    ExcelTipoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExcelNombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcelTipoDescripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    excelTipoPeriodo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcelTipos", x => x.ExcelTipoId);
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
                    ConfiguracionSuaNivelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfSuaNNombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfiguracionSuaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionSuaNiveles", x => x.ConfiguracionSuaNivelId);
                    table.ForeignKey(
                        name: "FK_ConfiguracionSuaNiveles_ConfiguracionSuas_ConfiguracionSuaId",
                        column: x => x.ConfiguracionSuaId,
                        principalTable: "ConfiguracionSuas",
                        principalColumn: "ConfiguracionSuaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExcelColumnas",
                columns: table => new
                {
                    ExcelColumnaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExcelColumnaNombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcelTipoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcelColumnas", x => x.ExcelColumnaId);
                    table.ForeignKey(
                        name: "FK_ExcelColumnas_ExcelTipos_ExcelTipoId",
                        column: x => x.ExcelTipoId,
                        principalTable: "ExcelTipos",
                        principalColumn: "ExcelTipoId",
                        onDelete: ReferentialAction.Cascade);
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
                    ConfiguracionSuaNivelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuaExcels", x => x.SuaExcelId);
                    table.ForeignKey(
                        name: "FK_SuaExcels_ConfiguracionSuaNiveles_ConfiguracionSuaNivelId",
                        column: x => x.ConfiguracionSuaNivelId,
                        principalTable: "ConfiguracionSuaNiveles",
                        principalColumn: "ConfiguracionSuaNivelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SuaExcels_ExcelColumnas_ExcelColumnaId",
                        column: x => x.ExcelColumnaId,
                        principalTable: "ExcelColumnas",
                        principalColumn: "ExcelColumnaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmpleadoColumnas",
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
                    table.PrimaryKey("PK_EmpleadoColumnas", x => x.EmpleadoColumnaId);
                    table.ForeignKey(
                        name: "FK_EmpleadoColumnas_SuaExcels_SuaExcelId",
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
                        name: "FK_EmpleadoColumnaV_EmpleadoColumnas_EmpleadoColumnaId",
                        column: x => x.EmpleadoColumnaId,
                        principalTable: "EmpleadoColumnas",
                        principalColumn: "EmpleadoColumnaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionSuaNiveles_ConfiguracionSuaId",
                table: "ConfiguracionSuaNiveles",
                column: "ConfiguracionSuaId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoColumnas_SuaExcelId",
                table: "EmpleadoColumnas",
                column: "SuaExcelId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoColumnaV_EmpleadoColumnaId",
                table: "EmpleadoColumnaV",
                column: "EmpleadoColumnaId");

            migrationBuilder.CreateIndex(
                name: "IX_ExcelColumnas_ExcelTipoId",
                table: "ExcelColumnas",
                column: "ExcelTipoId");

            migrationBuilder.CreateIndex(
                name: "IX_SuaExcels_ConfiguracionSuaNivelId",
                table: "SuaExcels",
                column: "ConfiguracionSuaNivelId");

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
                name: "EmpleadoColumnas");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "SuaExcels");

            migrationBuilder.DropTable(
                name: "ConfiguracionSuaNiveles");

            migrationBuilder.DropTable(
                name: "ExcelColumnas");

            migrationBuilder.DropTable(
                name: "ConfiguracionSuas");

            migrationBuilder.DropTable(
                name: "ExcelTipos");
        }
    }
}
