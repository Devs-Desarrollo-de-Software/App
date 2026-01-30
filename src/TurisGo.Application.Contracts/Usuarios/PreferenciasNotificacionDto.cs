using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisGo.Usuarios
{
    public class PreferenciasNotificacionDto
    {
        public bool RecibirEnPantalla { get; set; }
        public bool RecibirPorEmail { get; set; }
        public FrecuenciaNotificacion Frecuencia { get; set; }
    }
}
