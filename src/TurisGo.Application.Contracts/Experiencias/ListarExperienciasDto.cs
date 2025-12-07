using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisGo.Experiencias
{
    public class ListarExperienciasDto
    {
        public Guid DestinoId { get; set; }
        public List<ExperienciaPropiaDto> Experiencias { get; set; }

    }
}
