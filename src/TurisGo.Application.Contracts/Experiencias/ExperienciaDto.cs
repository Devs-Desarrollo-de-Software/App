using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace TurisGo.Experiencias
{
    public class ExperienciaDto : AuditedEntityDto<Guid>
    {
        public Guid UserId { get; set; }
        public Guid DestinoId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public TipoValoracion Valoracion { get; set; }
    }
}
