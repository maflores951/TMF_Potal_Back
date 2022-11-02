using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tmf_group.Models;

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
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Recibo> Recibos { get; set; }
        public DbSet<PeriodoTipo> PeriodoTipos { get; set; }

    }
}
