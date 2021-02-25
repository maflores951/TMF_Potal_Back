using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoginBase.Models.Empleado
{
    public class EmpleadoColumnaV
    {
        [Key]
        public int EmpleadoColumnaVId { get; set; }

        public string EmpleadoColumnaValor { get; set; }

        public int EmpleadoColumnaId { get; set; }

        public  EmpleadoColumna EmpleadoColumna { get; set; }
    }
}
