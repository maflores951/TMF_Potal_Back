
using LoginBase.Models.Menu;
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

        public DbSet<Funcion> Funciones { get; set; }
        public DbSet<RolModulo> RolModulos { get; set; }
        public DbSet<RolModFun> RolModFuns { get; set; }


    }
}
