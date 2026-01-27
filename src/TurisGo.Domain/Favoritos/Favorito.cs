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
    // Implementa IUserOwned para filtros automáticos por usuario
    public class Favorito : AuditedAggregateRoot<Guid>, IUserOwned
    {
        public Guid UserId { get; set; }
        public Guid DestinoId { get; set; }
        public virtual Destino Destino { get; set; }        // Propiedad de navegación hacia la entidad Destino (carga diferida)

        
        protected Favorito() { }    // Constructor protegido para EF Core


        // Constructor para crear un nuevo favorito
        public Favorito(Guid id,  Guid usuarioId, Guid destinoId) : base(id)
        {
             UserId = usuarioId;
             DestinoId = destinoId;
        }
    }
}
