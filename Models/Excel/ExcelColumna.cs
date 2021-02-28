using LoginBase.Models.Empleado;
using LoginBase.Models.Sua;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoginBase.Models.Excel
{
    public class ExcelColumna
    {
        [Key]
        public int ExcelColumnaId { get; set; }

        public string ExcelColumnaNombre { get; set; }

        public int ExcelTipoId { get; set; }
        [JsonIgnore]
        public ExcelTipo ExcelTipo { get; set; }
        [JsonIgnore]
        public List<SuaExcel> SuaExcel { get; set; }

        public ExcelColumna()
        {
            this.SuaExcel = new List<SuaExcel>();
        }
    }
}
