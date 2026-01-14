using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisGo.Calificaciones
{
    public class ComentarioDto
    {
        public int Puntuacion { get; set; }
        public string? Comentario { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
