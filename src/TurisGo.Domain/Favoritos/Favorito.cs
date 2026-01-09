using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisGo.Destinos;
using TurisGo.Usuarios;
using Volo.Abp.Domain.Entities.Auditing;

namespace TurisGo.Favoritos
{
    public class Favorito : AuditedAggregateRoot<Guid>, IUserOwned
    {
        public Guid UserId { get; set; }   // Para filtrar por usuario
        public Guid DestinoId { get; set; }

        protected Favorito() { } // Constructor protegido para EF Core

        public Favorito(Guid id,  Guid usuarioId, Guid destinoId) : base(id)
        {
             UserId = usuarioId;
             DestinoId = destinoId;
        }

    }
}
