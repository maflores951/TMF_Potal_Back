using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoginBase.Models.Excel
{
    public class ExcelTipo
    {
        [Key]
        public int ExcelTipoId { get; set; }

        public string ExcelNombre { get; set; }

        public string ExcelTipoDescripcion { get; set; }

        public  List<ExcelColumna> ExcelColumna { get; set; }

        public ExcelTipo()
        {
            this.ExcelColumna = new List<ExcelColumna>();
        }
    }
}
