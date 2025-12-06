using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisGo.Experiencias
{
    public class UpdateExperienciaDto
    {
        [Required]
        [MaxLength(100, ErrorMessage= "El titulo no debe superar los 100 caracteres.")]
        public string Titulo { get; set; }

        [Required]
        [MaxLength(500, ErrorMessage = "La descripción no debe superar los 500 caracteres.")]
        public string Descripcion { get; set; }

        [Required]
        [Range(0, 2, ErrorMessage = "La valoración debe ser: 0 (Positiva), 1 (Neutra) o 2 (Negativa).")]
        public TipoValoracion Valoracion { get; set; }

    }
}
