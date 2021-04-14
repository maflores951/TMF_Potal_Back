using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoginBase.Models.Excel
{
    public class ExcelComparativo
    {
        [Key]
        public int excelComparativoId { get; set; }

        public string excelComparativoNombre { get; set; }

        public int excelComparativoMes { get; set; }

        public int excelComparativoAnio { get; set; }

        public int excelTipoId { get; set; }

        public int UsuarioId { get; set; }

        public int ExcelTipoPeriodo { get; set; }
    }
}
