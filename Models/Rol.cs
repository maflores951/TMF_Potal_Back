using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LoginBase.Models
{
    public class Rol
    {
        [Key]
        public int RolId { get; set; }

        public string RolNombre { get; set; }

        [Display(Name = "Borrado logico")]
        public bool? RolEstatus { get; set; }

        [JsonIgnore]
        [NotMapped]
        public virtual List<Usuario> Usuarios { get; set; }

        public Rol()
        {
            this.Usuarios = new List<Usuario>();
        }
    }
}
