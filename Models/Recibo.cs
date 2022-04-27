using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tmf_group.Models
{
    public class Recibo
    {
        public string ReciboId { get; set; }

        public int ReciboPeriodoA { get; set; }

        public int ReciboPeriodoM { get; set; }

        public int ReciboPeriodoD { get; set; }

        public int ReciboPeriodoNumero { get; set; }

        public bool ReciboEstatus { get; set; }

        public int PeriodoTipoId { get; set; }

        public PeriodoTipo PeriodoTipo { get; set; }

        public string ReciboPath { get; set; }

        public string UsuarioNoEmp { get; set; }

        public int EmpresaId { get; set; }

        public Empresa Empresa { get; set; }

    }
}
