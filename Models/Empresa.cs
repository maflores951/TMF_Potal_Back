using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tmf_group.Models
{
    public class Empresa
    {
        public int EmpresaId { get; set; }

        public string EmpresaNombre { get; set; }

        public string EmpresaLogo { get; set; }

        [NotMapped]
        public string EmpresaImageBase64 { get; set; }

        public string EmpresaColor { get; set; }

        public bool EmpresaEstatus { get; set; }
    }
}
