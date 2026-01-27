using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisGo.Usuarios
{
    public class ActualizarPreferenciasDto
    {
        [Required(ErrorMessage = "Debe indicar si desea recibir notificaciones en pantalla")]
        public bool RecibirEnPantalla { get; set; }

        [Required(ErrorMessage = "Debe indicar si desea recibir notificaciones por mail")]
        public bool RecibirPorEmail { get; set; }

        [Required(ErrorMessage = "LLa frecuencia de notificación es requerida")]
        public FrecuenciaNotificacion Frecuencia { get; set; }

    }
}
