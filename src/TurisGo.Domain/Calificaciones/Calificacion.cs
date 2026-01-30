using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using TurisGo.Usuarios;

namespace TurisGo.Calificaciones
{
    // Implementa IUserOwned para filtros automáticos por usuario en ABP
    public class Calificacion : AuditedAggregateRoot<Guid>, IUserOwned
    {
        public int Puntuacion { get; private set; }     // Puntuación del destino (1 a 5 estrellas)
        public string? Comentario { get; private set; }
        public Guid DestinoId { get; private set; }     // ID del destino que está siendo calificado
        public Guid UserId { get;  set; }       // ID del usuario que creó la calificación (requerido por IUserOwned)


        
        protected Calificacion() { }        // Constructor protegido para EF Core

        public Calificacion(Guid id, Guid destinoId, Guid usuarioId, int puntuacion, string comentario) : base(id)
        {
            SetPuntuacion(puntuacion);
            SetComentario(comentario);
            SetDestino(destinoId);
            SetUser(usuarioId);
        }

        // Establece la puntuación validando que esté en el rango de 1 a 5
        public void SetPuntuacion(int puntuacion)
        {
            if (puntuacion < 1 || puntuacion > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(puntuacion),
                    "La puntuacion debe estar entre 1 y 5.");
            }
            Puntuacion = puntuacion;
        }

        // Establece el comentario validando su longitud máxima
        public void SetComentario(string comentario)
        {
            if (!string.IsNullOrEmpty(comentario) && comentario.Length > 1000)
            {
                throw new ArgumentException(
                    "El comentario no debe superar los 1000 caracteres.");
            }

            Comentario = string.IsNullOrWhiteSpace(comentario) ? null : comentario.Trim();
        }

        // Establece el destino asociado a la calificación
        public void SetDestino (Guid IdDestino)
        {
            if (IdDestino == Guid.Empty)
            {
                throw new ArgumentException(
                    "El ID del destino no puede estar vacio.", nameof(IdDestino));
            }

            DestinoId = IdDestino;
        }

        // Establece el usuario que creó la calificación
        public void SetUser(Guid IdUser)
        {
            if (IdUser == Guid.Empty)
            {
                throw new ArgumentException(
                    "El ID del usuario no puede estar vacio.", nameof(IdUser));
            }

            UserId = IdUser;
        }
    }
}
