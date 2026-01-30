using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisGo.Calificaciones
{
    public class UpdateCalificacionDto
    {
        [Required]
        [Range(1,5,ErrorMessage ="La puntuación debe estar entre 1 y 5.")]
        public int Puntuacion {  get; set; }

        [MaxLength(1000)]
        public string? Comentario { get; set; }
    }
}
