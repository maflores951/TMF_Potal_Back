using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoginBase.Models.Menu
{
    public class RolModulo
    {

        [Key]
        public int RolModuloId { get; set; }

        public int RolId { get; set; }

        public string RolModuloDesc { get; set; }

        public int RolModuloPosicion { get; set; }

        public bool RolModuloActivo { get; set; }

        public Rol Rol { get; set; }
    }
}
