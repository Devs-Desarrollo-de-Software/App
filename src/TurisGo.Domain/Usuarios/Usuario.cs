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

        // Referencia al usuario en la tabla de identidad de ABP (AbpUsers)
        // Permite la integración con el sistema de autenticación y autorización
        public Guid IdentityUserId { get; private set; }
        public string Email { get; private set; }
        public string? FotoPerfilUrl { get; private set; }      // URL de la foto de perfil
        public PreferenciasNotificacion Preferencias { get; private set; }
        public TipoRol Rol { get; private set; }        // Rol del usuario en el sistema (Usuario o Administrador)
        public bool EstaActivo { get; private set; }        // Indica si el usuario está activo o ha sido desactivado (soft delete)

        
        protected Usuario()     // Constructor protegido para EF Core
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

        // Establece el nombre completo del usuario con validaciones
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

        // Establece el nombre de usuario con validaciones
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

        // Establece el email con validación básica de formato
        public void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new AbpValidationException("El email no puede estar vacio.");
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                throw new AbpValidationException("El email no es válido.");
            }

            // Normaliza el email a minúsculas
            Email = email.Trim().ToLowerInvariant();
        }

        // Establece el rol del usuario validando que sea un valor válido del enum
        public void SetTipoRol(TipoRol rol)
        {
            if (!Enum.IsDefined(typeof(TipoRol), rol))
            {
                throw new AbpValidationException("El rol especificado no es válido.");
            }
            Rol = rol;
        }

        // Actualiza las preferencias de notificación del usuario
        public void ActualizarPreferencias(PreferenciasNotificacion nuevasPreferencias)
        {
            Check.NotNull(nuevasPreferencias, nameof(nuevasPreferencias));

            Preferencias.RecibirEnPantalla = nuevasPreferencias.RecibirEnPantalla;
            Preferencias.RecibirPorEmail = nuevasPreferencias.RecibirPorEmail;
            Preferencias.Frecuencia = nuevasPreferencias.Frecuencia;
        }

        // Establece la URL de la foto de perfil
        // Soporta URLs largas (Base64) por lo que no hay validación de longitud
        public void SetFotoPerfil(string? fotoPerfilUrl)
        {
            FotoPerfilUrl = fotoPerfilUrl?.Trim();
        }

        // Marca el usuario como inactivo (soft delete)
        public void Desactivar()
        {
            EstaActivo = false;
        }

        // Reactiva un usuario previamente desactivado
        public void Activar()
        {
            EstaActivo = true;
        }

        // Método alternativo para establecer el rol (delegado a SetTipoRol)
        public void SetRol(TipoRol rol)
        {
            SetTipoRol(rol);
        }
    }
}
