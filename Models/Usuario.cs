using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tmf_group.Models;

namespace LoginBase.Models
{
    //[Index(nameof(Email), IsUnique = true)]
    public class Usuario 
    {
        [Key]
        public int UsuarioId { get; set; }

        //[Display(Name = "Nombres(s)")]
        //[Required(ErrorMessage = "The field {0} is requiered.")]
        //[MaxLength(70, ErrorMessage = "The field {0} only can contains a maximum of {1} characters lenght.")]
        public string UsuarioNombre { get; set; }

        //[Display(Name = "Apellido paterno")]
        //[Required(ErrorMessage = "The field {0} is requiered.")]
        //[MaxLength(50, ErrorMessage = "The field {0} only can contains a maximum of {1} characters lenght.")]
        public string UsuarioApellidoP { get; set; }

        //[Display(Name = "Apellido materno")]
        //[Required(ErrorMessage = "The field {0} is requiered.")]
        //[MaxLength(50, ErrorMessage = "The field {0} only can contains a maximum of {1} characters lenght.")]
        public string UsuarioApellidoM { get; set; }

        public int? UsuarioNumConfirmacion { get; set; }

        public DateTime? UsuarioFechaLimite { get; set; }

        public bool? UsuarioEstatusSesion { get; set; }

        //[Required(ErrorMessage = "The field {0} is requiered.")]
        //[MaxLength(100, ErrorMessage = "The field {0} only can contains a maximum of {1} characters lenght.")]
        
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailSSO { get; set; }

        public string UsuarioClave { get; set; }

        public string UsuarioToken { get; set; }

        [Display(Name = "Password")]
        //[Required(ErrorMessage = "The field {0} is requiered.")]
        //[MaxLength(256, ErrorMessage = "The field {0} only can contains a maximum of {1} characters lenght.")]
        //[MinLength(6, ErrorMessage = "The field {0} only can contains a minimum of {1} characters lenght.")]
        public string Password { get; set; }

        //[MaxLength(20, ErrorMessage = "The field {0} only can contains a maximum of {1} characters lenght.")]

        [Display(Name = "Image")]
        public string ImagePath { get; set; }

        public int? RolId { get; set; }

        public string EmpleadoNoEmp { get; set; }

        //public string EmpleadoRFC { get; set; }

        public int? EmpresaId { get; set; }

        //[JsonIgnore]
        //[NotMapped]
        public  Rol Rol { get; set; }

        public Empresa Empresa { get; set; }

        [NotMapped]
        [JsonIgnore]
        public byte[] ImageArray { get; set; }

        [NotMapped]
        public string ImageBase64 { get; set; }

        //[NotMapped]
        //public string Password { get; set; }

        [Display(Name = "Image")]
        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImagePath))
                {
                    return "noimage";
                }

                return string.Format(
                    "http://legvit.ddns.me/Fintech_Api/{0}",
                    ImagePath.Substring(1));
            }
        }

        [Display(Name = "Usuario")]
        public string UsuarioFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", this.UsuarioNombre, this.UsuarioApellidoP, this.UsuarioApellidoM);
            }
        }
    }
}
