using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoginBase.Models.Sua
{
    public class ConfiguracionSuaNivel
    {
        [Key]
        public int ConfiguracionSuaNivelId { get; set; }

        public string ConfSuaNNombre { get; set; }

        public int ConfiguracionSuaId { get; set; }

        public  ConfiguracionSua ConfiguracionSua { get; set; }

        public  List<SuaExcel> SuaExcel { get; set; }

        public ConfiguracionSuaNivel()
        {
            this.SuaExcel = new List<SuaExcel>();
        }
    }
}
