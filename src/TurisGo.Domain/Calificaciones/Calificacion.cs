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
            SetPutuacion(puntuacion);
            SetComentario(comentario);
            SetDestino(destinoId);
            SetUser(usuarioId);
        }

        public void SetPutuacion(int puntuacion)
        {
            if (puntuacion < 1 || puntuacion > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(puntuacion),
                    "La puntuacion debe estar entre 1 y 5.");
            }
            Puntuacion = puntuacion;
        }

        public void SetComentario(string comentario)
        {
            if (!string.IsNullOrEmpty(comentario) && comentario.Length > 1000)
            {
                throw new ArgumentException(
                    "El comentario no debe superar los 1000 caracteres.");
            }

            Comentario = string.IsNullOrWhiteSpace(comentario) ? null : comentario.Trim();
        }

        public void SetDestino (Guid IdDestino)
        {
            if (IdDestino == Guid.Empty)
            {
                throw new ArgumentException(
                    "El ID del destino no puede estar vacio.", nameof(IdDestino));
            }

            DestinoId = IdDestino;
        }

        public void SetUser(Guid IdUser)
        {
            if (IdUser == Guid.Empty)
            {
                throw new ArgumentException(
                    "El ID del destino no puede estar vacio.", nameof(IdUser));
            }

            UserId = IdUser;
        }



    }
}
