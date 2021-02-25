using LoginBase.Models.Empleado;
using LoginBase.Models.Excel;
using LoginBase.Models.Sua;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginBase.Models
{
    public class DataContext : DbContext
    {


        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
             
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Parametro> Parametros { get; set; }

        public DbSet<ConfiguracionSua> ConfiguracionSuas { get; set; }

        public DbSet<ConfiguracionSuaNivel> ConfiguracionSuaNiveles { get; set; }

        public DbSet<SuaExcel> SuaExcels { get; set; }

        public DbSet<EmpleadoColumna> EmpleadoColumnas { get; set; }

        public DbSet<EmpleadoColumnaV> EmpleadoColumnaV { get; set; }

        public DbSet<ExcelColumna> ExcelColumnas { get; set; }

        public DbSet<ExcelTipo> ExcelTipos { get; set; }
    }
}
