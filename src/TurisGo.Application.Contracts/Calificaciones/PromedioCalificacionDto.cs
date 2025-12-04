using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisGo.Calificaciones
{
    public class PromedioCalificacionDto
    {
        public Guid DestinoId { get; set; }
        public double PromedioCalificacion {  get; set; }
        public int TotalCalificaciones { get; set; }

    }
}
