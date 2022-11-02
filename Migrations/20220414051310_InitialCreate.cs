using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tmf_group.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaNombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmpresaLogo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmpresaColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmpresaEstatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.EmpresaId);
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
                name: "PeriodoTipos",
                columns: table => new
                {
                    PeriodoTipoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PeriodoTipoNombre = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodoTipos", x => x.PeriodoTipoId);
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
                name: "Recibos",
                columns: table => new
                {
                    ReciboId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReciboPeriodoA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReciboPeriodoM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReciboPeriodoD = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReciboEstatus = table.Column<bool>(type: "bit", nullable: false),
                    PeriodoTipoId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PeriodoTipoId1 = table.Column<int>(type: "int", nullable: true),
                    ReciboPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioNoEmp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recibos", x => x.ReciboId);
                    table.ForeignKey(
                        name: "FK_Recibos_PeriodoTipos_PeriodoTipoId1",
                        column: x => x.PeriodoTipoId1,
                        principalTable: "PeriodoTipos",
                        principalColumn: "PeriodoTipoId",
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
                    UsuarioClave = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RolId = table.Column<int>(type: "int", nullable: true),
                    EmpleadoNoEmp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmpresaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioId);
                    table.ForeignKey(
                        name: "FK_Usuarios_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "RolId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recibos_PeriodoTipoId1",
                table: "Recibos",
                column: "PeriodoTipoId1");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EmpresaId",
                table: "Usuarios",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolId",
                table: "Usuarios",
                column: "RolId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Parametros");

            migrationBuilder.DropTable(
                name: "Recibos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "PeriodoTipos");

            migrationBuilder.DropTable(
                name: "Empresas");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
