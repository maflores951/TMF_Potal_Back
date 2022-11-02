using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoginBase.Models.Menu
{
    public class RolModFun
    {


        [Key]
        public int RolModFunId { get; set; }

        public int RolModuloId { get; set; }

        public int RolId{ get; set; }

        public int FuncionEnMenu { get; set; }

        public int FuncionPosicion { get; set; }

        public RolModulo RolModulo { get; set; }
    }
}
