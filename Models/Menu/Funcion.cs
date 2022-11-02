using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoginBase.Models.Menu
{
    public class Funcion
    {


        [Key]
        public int FuncionId { get; set; }

        public string FuncionNombre { get; set; }

        public int FuncionPadre { get; set; }

        public string FuncionRuta { get; set; }

        public bool FuncionMostrar { get; set; }

        public bool FuncionActivo { get; set; }
    }
}