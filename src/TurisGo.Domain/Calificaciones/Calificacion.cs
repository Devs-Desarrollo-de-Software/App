using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using TurisGo.Usuarios;

namespace TurisGo.Calificaciones
{
    public class Calificacion : AuditedAggregateRoot<Guid>, IUserOwned
    {
        public int Puntuacion { get; private set; } // de 1 a 5
        public string? Comentario { get; private set; }
        public Guid DestinoId { get; private set; }         
        public Guid UserId { get;  set; }   // Para filtrar por usuario

        protected Calificacion() { } // Constructor protegido para EF Core

        public Calificacion(Guid id, Guid destinoId, Guid usuarioId, int puntuacion, string comentario) : base(id)
        {
            Puntuacion = puntuacion;
            Comentario = string.IsNullOrWhiteSpace(comentario) ? null : comentario.Trim();
            DestinoId = destinoId;
            UserId = usuarioId;

            if (puntuacion < 1 || puntuacion > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(puntuacion), "La puntuación debe estar entre 1 y 5.");
            }

            if (DestinoId == Guid.Empty)
            {
                throw new ArgumentException("El ID del destino no puede estar vacío.", nameof(DestinoId));
            }

            if (UserId == Guid.Empty)
            {
                throw new ArgumentException("El ID del usuario no puede estar vacío.", nameof(UserId));
            }

            if (!string.IsNullOrEmpty(comentario) && comentario.Length > 1000)
                throw new ArgumentException("El comentario no puede superar los 1000 caracteres.", nameof(comentario));

         
        }

    }
}
