using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace TurisGo.Experiencias
{
    public class GetExperienciasListDto : PagedAndSortedResultRequestDto
    {
        public TipoValoracion? Valoracion { get; set; }
    }
}
