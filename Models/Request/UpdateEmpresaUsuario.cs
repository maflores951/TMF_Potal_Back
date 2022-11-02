using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tmf_group.Models.Request
{
    public class UpdateEmpresaUsuario
    {
        public string EmpleadoNoEmp { get; set; }

        public int EmpresaIdOld { get; set; }

        public int EmpresaIdNew { get; set; }
    }
}
