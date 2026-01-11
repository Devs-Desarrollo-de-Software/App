using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisGo.Notificaciones{
    public class NotificacionResultDto
    {
        public int UsuariosNotificados {  get; set; }
        public string NombreDestino { get; set; }
        public TipoNotificacion Tipo {  get; set; }
        public string Mensaje { get; set; }
    }
}
