using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace TurisGo.Calificaciones
{
    public class CalificacionDto : AuditedEntityDto<Guid>
    {
        public int Puntuacion { get; set; }
        public string? Comentario { get; set; }
        public Guid DestinoId { get; set; }
        public Guid UserId { get; set; }                        
    }
}
