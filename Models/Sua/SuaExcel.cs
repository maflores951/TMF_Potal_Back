using LoginBase.Models.Empleado;
using LoginBase.Models.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoginBase.Models.Sua
{
    public class SuaExcel
    {
        [Key]
        public int SuaExcelId { get; set; }

        public int TipoPeriodoId { get; set; }

        public int ExcelColumnaId { get; set; }

        public  ExcelColumna ExcelColumna { get; set; }

        public int ConfiguracionSuaNivelId { get; set; }

        public  ConfiguracionSuaNivel ConfiguracionSuaNivel { get; set; }

        public  List<EmpleadoColumna> EmpleadoColumna { get; set; }

        public SuaExcel()
        {
            this.EmpleadoColumna = new List<EmpleadoColumna>();
        }
    }
}
