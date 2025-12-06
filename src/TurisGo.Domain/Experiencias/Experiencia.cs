using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Validation;

namespace TurisGo.Experiencias
{
    public class Experiencia : AuditedAggregateRoot<Guid>
    {
        public Guid UserId { get; set; }
        public Guid DestinoId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public TipoValoracion Valoracion { get; set; }

        protected Experiencia () { }  // Constructor protegido para EF Core

        public Experiencia (Guid id, Guid userId, Guid destinoId, string titulo, string descripcion, TipoValoracion valoracion)
            : base(id)
        {
            SetUserId(userId);
            SetDestinoId(destinoId);
            SetTitulo(titulo);
            SetDescripcion(descripcion);
            SetValoracion(valoracion);
        }

        public void SetUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new AbpValidationException("El ID del usuario no puede estar vacío.");
            }

            UserId = userId;
        }

        public void SetDestinoId (Guid destinoId)
        {
            if (destinoId == Guid.Empty)
            {
                throw new AbpValidationException("El ID del usuario no puede estar vacío.");
            }

            DestinoId = destinoId;
        }

        public void SetTitulo (string titulo)
        {
            if (string.IsNullOrEmpty(titulo))
            {
                throw new AbpValidationException("El titulo no debe ser nulo.");
            }
            if (titulo.Length > 100)
            {
                throw new AbpValidationException("El titulo no debe exceder de los 100 caracteres.");
            }

            Titulo = titulo;
        }

        public void SetDescripcion (string descripcion)
        {
            if (string.IsNullOrEmpty(descripcion))
            {
                throw new AbpValidationException("La descripcion no debe ser nula.");
            }
            if (descripcion.Length > 500)
            {
                throw new AbpValidationException("La descripcion no debe exceder los 200 caracteres.");
            }

            Descripcion = descripcion;
        }

        public void SetValoracion (TipoValoracion valoracion)
        {
            if (!Enum.IsDefined(typeof(TipoValoracion), valoracion))
            {
                throw new ArgumentException("Valoracion inválida.", nameof(valoracion));
            }

            Valoracion = valoracion;
        }

    }
}
