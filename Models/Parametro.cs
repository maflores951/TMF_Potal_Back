using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoginBase.Models
{
    public class Parametro
    {
        [Key]
        public int ParametroId { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo  {1} caracteres.")]
        public string ParametroNombre { get; set; }

        [Display(Name = "Clave")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(6, ErrorMessage = "El campo {0} debe tener máximo  {1} caracteres.")]
        public string ParametroClave { get; set; }

        [Display(Name = "Descripción")]
        [MaxLength(70, ErrorMessage = "El campo {0} debe tener máximo  {1} caracteres.")]
        public string ParametroDescripcion { get; set; }

        [Display(Name = "Valor inicial")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo  {1} caracteres.")]
        public string ParametroValorInicial { get; set; }

        [Display(Name = "Valor final")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo  {1} caracteres.")]
        public string ParametroValorFinal { get; set; }

        [Display(Name = "Borrado logico")]
        public bool? ParametroEstatusDelete { get; set; }
    }
}
