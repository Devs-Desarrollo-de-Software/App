using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Validation;
using Xunit;

namespace TurisGo.Usuarios
{
    public class Usuario_Tests
    {

        // Helper method
        private Usuario CreateValidUsuario()
        {
            return new Usuario(
                Guid.NewGuid(),
                "Juan Pérez",
                "juanperez",
                Guid.NewGuid(),
                "juan@test.com",
                TipoRol.Usuario
            );
        }

        [Fact]
        public void Constructor_WithValidData_CreateUsuariosSuccessfully()
        {

            // Arrange
            var id = Guid.NewGuid();
            var identityUserId = Guid.NewGuid();
            var nombreCompleto = "Juan Perez";
            var nombreUsuario = "juanperez";
            var email = "juan@test.com";
            var rol = TipoRol.Usuario;

            // Act
            var usuario = new Usuario(
                id,
                nombreCompleto,
                nombreUsuario,
                identityUserId,
                email,
                rol
            );  

            // Assert
            usuario.Id.ShouldBe(id);
            usuario.NombreCompleto.ShouldBe(nombreCompleto);
            usuario.NombreUsuario.ShouldBe(nombreUsuario);
            usuario.IdentityUserId.ShouldBe(identityUserId);
            usuario.Email.ShouldBe(email);
            usuario.Rol.ShouldBe(rol);
            usuario.EstaActivo.ShouldBeTrue();
            usuario.Preferencias.ShouldNotBeNull();
            usuario.Preferencias.RecibirEnPantalla.ShouldBeTrue();
            usuario.Preferencias.RecibirPorEmail.ShouldBeFalse();
            usuario.Preferencias.Frecuencia.ShouldBe(FrecuenciaNotificacion.Inmediata);
        }

        [Fact]
        public void SetNombreCompleto_WithEmptyName_ThrowsValidationException()
        {

            var usuario = CreateValidUsuario();

            Should.Throw<AbpValidationException>(() => 
            usuario.SetNombreCompleto(""));
        }

        [Fact]
        public void SetNombreCompleto_WithWhitespaceName_ThrowsValidationException()
        {
            // Arrange
            var usuario = CreateValidUsuario();

            // Act & Assert
            Should.Throw<AbpValidationException>(() =>
                usuario.SetNombreCompleto("   ")
            );
        }

        [Fact]
        public void SetNombreUsuario_WithEmptyUsername_ThrowsValidationException()
        {
            // Arrange
            var usuario = CreateValidUsuario();

            // Act & Assert
            Should.Throw<AbpValidationException>(() =>
                usuario.SetNombreUsuario("")
            );
        }

        [Fact]
        public void ActualizarPreferencias_WithNullPreferences_ThrowsValidationException()
        {
            // Arrange
            var usuario = CreateValidUsuario();

            // Act & Assert
            Should.Throw<AbpValidationException>(() =>
                usuario.ActualizarPreferencias(null)
            );
        }

        [Fact]
        public void SetFotoPerfil_WithNull_ClearsPhoto()
        {
            // Arrange
            var usuario = CreateValidUsuario();
            usuario.SetFotoPerfil("https://example.com/photo.jpg");

            // Act
            usuario.SetFotoPerfil(null);

            // Assert
            usuario.FotoPerfilUrl.ShouldBeNull();
        }

        [Fact]
        public void SetEmail_WithNullEmail_ThrowsValidationException()
        {
            // Arrange
            var usuario = CreateValidUsuario();

            // Act & Assert
            Should.Throw<AbpValidationException>(() =>
                usuario.SetEmail(null)
            );
        }
    }
    
}
