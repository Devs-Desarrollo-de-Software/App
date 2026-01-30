using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace TurisGo.Destinos
{
    public class DestinoDto : AuditedEntityDto<Guid>
    {
        public string? Nombre { get; set; }
        public string? Pais { get; set; }
        public int Poblacion { get; set; }
        public string? Imagen { get; set; }
        public CoordenadaDto Coordenada { get; set; }
        public int ApiCityId { get; set; }
    }
}
