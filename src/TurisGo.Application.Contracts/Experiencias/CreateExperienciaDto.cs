using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisGo.Experiencias
{
    public class CreateExperienciaDto
    {
        [Required]
        public Guid DestinoId { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "El título no debe superar los 100 caracteres.")]
        public string Titulo { get; set; }

        [Required]
        [MaxLength(500, ErrorMessage = "La descripción no debe superar los 500 caracteres.")]
        public string Descripcion { get; set; }

        [Required]
        public TipoValoracion Valoracion { get; set; }

    }
}
