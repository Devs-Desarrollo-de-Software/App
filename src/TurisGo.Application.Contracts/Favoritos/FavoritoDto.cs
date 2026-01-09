using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace TurisGo.Favoritos
{
    public class FavoritoDto : EntityDto<Guid>
    {
        public Guid UserId { get; set; }
        public Guid DestinoId { get; set; }
    }
}
