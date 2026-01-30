using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace TurisGo.Notificaciones
{
    public class NotificacionDto : EntityDto<Guid>
    {
        public Guid DestinoId { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public TipoNotificacion Tipo { get; set; }
        public bool Leida { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string NombreDestino { get;  set; }
    }
}
