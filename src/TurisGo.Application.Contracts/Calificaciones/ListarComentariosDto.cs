using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisGo.Calificaciones
{
    public class ListarComentariosDto
    {
        public Guid DestinoId { get; set; }
        public List<ComentarioDto> Comentarios { get; set; }

    }
}
