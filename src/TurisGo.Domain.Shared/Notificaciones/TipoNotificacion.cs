using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisGo.Notificaciones
{
    public enum TipoNotificacion
    {
        // Cambio en datos del destino (poblacion, nombre, region,etc.)
        CambioDestino = 1,

        // Nuevo evento disponible en el destino
        NuevoEvento = 2,

        // Actualizacion de datos geograficos del destino
        ActualizacionDatos = 3,

        // Notificacion del sistema
        Sistema = 4
    }
}
