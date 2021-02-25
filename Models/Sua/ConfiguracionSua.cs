using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoginBase.Models.Sua
{
    public class ConfiguracionSua
    {

        [Key]
        public int ConfiguracionSuaId { get; set; }

        public string ConfSuaNombre { get; set; }

        public bool ConfSuaEstatus { get; set; }

        public  List<ConfiguracionSuaNivel> ConfiguracionSuaNivel { get; set; }

        public ConfiguracionSua()
        {
            this.ConfiguracionSuaNivel = new List<ConfiguracionSuaNivel>();
        }
    }
}
