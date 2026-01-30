using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisGo.Calificaciones
{
    public class CreateCalificacionDto
    {
        [Required]
        public Guid DestinoId { get; set; }

        [Required]
        [Range(1,5, ErrorMessage = "La puntuacion debe estar entre 1 y 5.")]
        public int Puntuacion { get; set; }

        [MaxLength(1000)]
        public string? Comentario { get; set; }
    }
}
