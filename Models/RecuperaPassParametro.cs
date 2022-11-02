using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginBase.Models
{
    public class RecuperaPassParametro
    {
        public DateTime? UsuarioFechaLimite { get; set; }

        public string Email { get; set; }

        public int UsuarioId { get; set; }

        public string UsuarioClave { get; set; }
    }
}
