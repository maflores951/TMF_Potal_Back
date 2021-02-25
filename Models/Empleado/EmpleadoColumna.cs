using LoginBase.Models.Sua;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoginBase.Models.Empleado
{
    public class EmpleadoColumna
    {
        [Key]
        public int EmpleadoColumnaId { get; set; }

        public int EmpleadoColumnaMes { get; set; }

        public int EmpleadoColumnaAnio { get; set; }

        public int SuaExcelId { get; set; }

        public  SuaExcel SuaExcel { get; set; }

        public  List<EmpleadoColumnaV> EmpleadoColumnaV { get; set; }

        public EmpleadoColumna()
        {
            this.EmpleadoColumnaV = new List<EmpleadoColumnaV>();
        }
    }
}
