using LoginBase.Models.Empleado;
using LoginBase.Models.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [NotMapped]
        //[JsonIgnore]
        public int ExcelTipoId { get; set; }
        [NotMapped]
        //[JsonIgnore]
        public ExcelTipo ExcelTipo { get; set; }
        
        public int ConfiguracionSuaNivelId { get; set; }

        [NotMapped]
        public  ConfiguracionSuaNivel ConfiguracionSuaNivel { get; set; }

        //public  List<EmpleadoColumna> EmpleadoColumna { get; set; }

        public SuaExcel()
        {
            //this.EmpleadoColumna = new List<EmpleadoColumna>();
        }
    }
}
