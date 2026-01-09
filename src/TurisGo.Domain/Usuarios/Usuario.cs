using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Validation;

namespace TurisGo.Usuarios
{
    public class Usuario : FullAuditedAggregateRoot<Guid>
    {
        public string NombreCompleto { get; private set; }
        public string NombreUsuario { get; private set; }
        public Guid IdentityUserId { get; private set; }
        public string Email { get; private set; }
        public string? FotoPerfilUrl { get; private set; }
        public PreferenciasNotificacion Preferencias { get; private set; }
        public TipoRol Rol { get; private set; }
        public bool EstaActivo { get; private set; }

        // Constructor protegido para EF Core
        protected Usuario()
        {
            Preferencias = new PreferenciasNotificacion();
            EstaActivo = true;
        }

        public Usuario(Guid id, string nombreCompleto, string nombreUsuario, Guid identityUserId,
            string email, TipoRol rol, string? fotoPerfil = null) : base(id)
        {
            SetNombreCompleto(nombreCompleto);
            SetNombreUsuario(nombreUsuario);
            SetEmail(email);
            SetTipoRol(rol);
            Preferencias = new PreferenciasNotificacion();
            SetFotoPerfil(fotoPerfil);
            IdentityUserId = identityUserId;
            EstaActivo = true;

        }

        public void SetNombreCompleto(string nombreCompleto)
        {
            if (string.IsNullOrWhiteSpace(nombreCompleto))
            {
                throw new AbpValidationException("El nombre completo no puede estar vacío.");
            }

            if (nombreCompleto.Length > 100)
            {
                throw new AbpValidationException("El nombre completo debe tener menos de 100 caracteres.");
            }

            NombreCompleto = nombreCompleto.Trim();
        }

        public void SetNombreUsuario(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                throw new AbpValidationException("El nombre de usuario no puede estar vacío.");
            }

            if (nombreUsuario.Length > 50)
            {
                throw new AbpValidationException("El nombre de usuario debe tener menos de 50 caracteres.");
            }

            NombreUsuario = nombreUsuario.Trim();
        }

        public void SetEmail(string email)
        {
            // Validación básica de email
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new AbpValidationException("El email no puede estar vacio.");
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                throw new AbpValidationException("El email no es válido.");
            }

            Email = email.Trim().ToLowerInvariant();
        }

        public void SetTipoRol(TipoRol rol)
        {
            if (!Enum.IsDefined(typeof(TipoRol), rol))
            {
                throw new AbpValidationException("El rol especificado no es válido.");
            }
            Rol = rol;
        }

        public void ActualizarPreferencias(PreferenciasNotificacion nuevasPreferencias)
        {
            Check.NotNull(nuevasPreferencias, nameof(nuevasPreferencias));

            Preferencias.RecibirEnPantalla = nuevasPreferencias.RecibirEnPantalla;
            Preferencias.RecibirPorEmail = nuevasPreferencias.RecibirPorEmail;
            Preferencias.Frecuencia = nuevasPreferencias.Frecuencia;
        }

        public void SetFotoPerfil(string? fotoPerfilUrl)
        {
            if (fotoPerfilUrl?.Length > 500)
            {
                throw new AbpValidationException("La URL de la foto de perfil no puede exceder 500 caracteres.");
            }

            FotoPerfilUrl = fotoPerfilUrl?.Trim();

        }
    }
}
