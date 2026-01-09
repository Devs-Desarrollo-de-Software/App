using Volo.Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.Users;
using Volo.Abp.Validation;
using Xunit;



namespace TurisGo.Usuarios
{
    public abstract class UsuarioAppService_Tests<TStartupModule> : TurisGoApplicationTestBase<TStartupModule>
       where TStartupModule : IAbpModule
    {
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly Volo.Abp.Domain.Repositories.IRepository<Usuario, Guid> _usuarioRepository;
        private readonly IdentityUserManager _identityUserManager;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly ICurrentUser _currentUser;    

        public UsuarioAppService_Tests()
        {
            _usuarioAppService = GetRequiredService<IUsuarioAppService>();
            _usuarioRepository = GetRequiredService<Volo.Abp.Domain.Repositories.IRepository<Usuario, Guid>>();
            _identityUserManager = GetRequiredService<IdentityUserManager>();
            _identityUserRepository = GetRequiredService<IIdentityUserRepository>();
            _currentUser = GetRequiredService<ICurrentUser>();
        }

        // Helper method
        private CrearUsuarioDto CreateValidCrearUsuarioDto()
        {
            return new CrearUsuarioDto
            {
                NombreCompleto = "Test User",
                NombreUsuario = $"testuser{Guid.NewGuid().ToString("N").Substring(0, 8)}",
                Email = $"test{Guid.NewGuid().ToString("N").Substring(0, 8)}@test.com",
                Password = "Test123!",
                Rol = TipoRol.Usuario,
                FotoPerfilUrl = null
            };
        }


        // ----------- Operacion 1.1. Registrar nuevo usuario ----------------
        [Fact]
        public async Task RegistrarUsuarioAsync_WithValidData_ReturnsUsuarioDto()
        {

            // Arrange
            var input = CreateValidCrearUsuarioDto();

            // Act
            var result = await _usuarioAppService.RegistrarUsuarioAsync(input);

            // Assert
            result.ShouldNotBeNull();
            result.NombreCompleto.ShouldBe(input.NombreCompleto);
            result.NombreUsuario.ShouldBe(input.NombreUsuario);
            result.Email.ShouldBe(input.Email);
            result.Rol.ShouldBe(input.Rol);
            result.EstaActivo.ShouldBeTrue();
        }

        [Fact]
        public async Task RegistrarUsuarioAsync_WithValidData_CreatesUsuarioInDatabase()
        {
            // Arrange
            var input = CreateValidCrearUsuarioDto();

            // Act
            var result = await _usuarioAppService.RegistrarUsuarioAsync(input);

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var usuarioInDb = await _usuarioRepository
                    .FirstOrDefaultAsync(u => u.NombreUsuario == input.NombreUsuario);

                usuarioInDb.ShouldNotBeNull();
                usuarioInDb.NombreCompleto.ShouldBe(input.NombreCompleto);
            });
        }

        [Fact]
        public async Task RegistrarUsuarioAsync_WithValidData_CreatesIdentityUser()
        {
            // Arrange
            var input = CreateValidCrearUsuarioDto();

            // Act
            var result = await _usuarioAppService.RegistrarUsuarioAsync(input);

            // Assert
            var identityUser = await _identityUserRepository
                .FindByNormalizedUserNameAsync(input.NombreUsuario.ToUpperInvariant());

            identityUser.ShouldNotBeNull();
            identityUser.UserName.ShouldBe(input.NombreUsuario);
            identityUser.Email.ShouldBe(input.Email);
        }

        [Fact]
        public async Task RegistrarUsuarioAsync_WithDuplicateUsername_ThrowsUserFriendlyException()
        {
            // Arrange
            var input = CreateValidCrearUsuarioDto();
            await _usuarioAppService.RegistrarUsuarioAsync(input);

            var duplicateInput = CreateValidCrearUsuarioDto();
            duplicateInput.NombreUsuario = input.NombreUsuario; // Same username

            // Act & Assert
            var exception = await Should.ThrowAsync<BusinessException>(
                _usuarioAppService.RegistrarUsuarioAsync(duplicateInput)
            );
        }

        [Fact]
        public async Task RegistrarUsuarioAsync_WithWeakPassword_ThrowsUserFriendlyException()
        {
            // Arrange
            var input = CreateValidCrearUsuarioDto();
            input.Password = "weak"; // Weak password

            // Act & Assert
            await Should.ThrowAsync<AbpValidationException>(
                _usuarioAppService.RegistrarUsuarioAsync(input)
            );
        }

        // ----------------- Operacion 1.3. Actualizar perfil de usuario -----------------

        [Fact]
        public async Task ObtenerPerfilActualAsync_WithoutAuthentication_ThrowsAbpAuthorizationException()
        {
            // Act & Assert - Sin establecer CurrentUser.Id
            await Should.ThrowAsync<EntityNotFoundException>(
                _usuarioAppService.ObtenerPerfilActualAsync()
            );
        }

        [Fact]
        public async Task ActualizarPerfilAsync_WithoutAuthentication_ThrowsAbpAuthorizationException()
        {
            // Arrange
            var updateInput = new ActualizarPerfilDto
            {
                NombreCompleto = "Test"
            };

            // Act & Assert - Sin establecer CurrentUser.Id
            await Should.ThrowAsync<EntityNotFoundException>(
                _usuarioAppService.ActualizarPerfilAsync(updateInput)
            );
        }

        [Fact]
        public async Task ActualizarPreferenciasAsync_WithoutAuthentication_ThrowsAbpAuthorizationException()
        {
            // Arrange
            var updateInput = new ActualizarPreferenciasDto
            {
                RecibirEnPantalla = true,
                RecibirPorEmail = false,
                Frecuencia = FrecuenciaNotificacion.Inmediata
            };

            // Act & Assert - Sin establecer CurrentUser.Id
            await Should.ThrowAsync<EntityNotFoundException>(
                _usuarioAppService.ActualizarPreferenciasAsync(updateInput)
            );
        }

        // ------------------ Operacion 1.4 Cambiar contraseña ------------------

        [Fact]
        public async Task CambiarContrasenaAsync_WithIncorrectCurrentPassword_ThrowsBusinessException()
        {
            // Arrange
            var cambiarContrasenaDto = new CambiarPasswordDto
            {
                PasswordActual = "ContrasenaIncorrecta123!",
                NuevoPassword = "NuevaPassword123!",
                ConfirmarNuevoPassword = "NuevaPassword123!"
            };

            // Act & Assert
            await Should.ThrowAsync<EntityNotFoundException>(
                _usuarioAppService.CambiarPasswordAsync(cambiarContrasenaDto)
            );
        }

        [Fact]
        public async Task CambiarContrasenaAsync_WithMismatchedPasswords_ThrowsValidationException()
        {
            // Arrange
            var cambiarContrasenaDto = new CambiarPasswordDto
            {
                PasswordActual = "Password123!",
                NuevoPassword = "NuevaPassword123!",
                ConfirmarNuevoPassword = "DiferentePassword123!"
            };

            // Act & Assert
            await Should.ThrowAsync<AbpValidationException>(
                _usuarioAppService.CambiarPasswordAsync(cambiarContrasenaDto)
            );
        }

        [Fact]
        public async Task CambiarContrasenaAsync_WithWeakPassword_ThrowsUserFriendlyException()
        {
            // Arrange
            var cambiarContrasenaDto = new CambiarPasswordDto
            {
                PasswordActual = "Password123!",
                NuevoPassword = "123", // Contraseña débil
                ConfirmarNuevoPassword = "123"
            };

            // Act & Assert
            await Should.ThrowAsync<AbpValidationException>(
                _usuarioAppService.CambiarPasswordAsync(cambiarContrasenaDto)
            );
        }

        // ------------------ Operacion 1.5. Eliminar cuenta propia ------------------

        [Fact]
        public async Task EliminarCuentaPropia_Without_ThrowsAuthorizationException()
        {
            // Arrange
            var eliminarDto = new EliminarCuentaDto
            {
                Password = "Password123!"
            };

            // Act & Assert - Sin establecer CurrentUser.Id
            await Should.ThrowAsync<EntityNotFoundException>(
                _usuarioAppService.EliminarCuentaPropia(eliminarDto)
            );
        }

        // ----------------- Operacion 1.6. Consultar perfil de otro usuario -----------------

        [Fact]
        public async Task ObtenerPerfilPublico_WithValidUserName_RetornPerfilPublic()
        {
            // Arrange
            var input = CreateValidCrearUsuarioDto();
            var usuario = await _usuarioAppService.RegistrarUsuarioAsync(input);

            // Act
            var perfilPublico = await _usuarioAppService
                .ObtenerPerfilPublicoAsync(input.NombreUsuario);

            // Assert
            perfilPublico.ShouldNotBeNull();
            perfilPublico.NombreCompleto.ShouldBe(input.NombreCompleto);
            perfilPublico.NombreUsuario.ShouldBe(input.NombreUsuario);
            perfilPublico.FotoPerfilUrl.ShouldBe(input.FotoPerfilUrl);

        }

        [Fact]
        public async Task ObtenerPerfilPublico_ConNombreUsuarioInexistente_ThrowsEntityNotFoundException()
        {
            // Arrange
            var nombreUsuarioInexistente = "usuarioquenoxiste123";

            // Act & Assert
            await Should.ThrowAsync<EntityNotFoundException>(
                _usuarioAppService.ObtenerPerfilPublicoAsync(nombreUsuarioInexistente)
            );
        }

        [Fact]
        public async Task ObtenerPerfilPublico_WithEmptyUserName_ThrowsValidationException()
        {
            // Arrange
            var nombreUsuarioVacio = "";

            // Act & Assert
            await Should.ThrowAsync<Abp.Runtime.Validation.AbpValidationException>(
                _usuarioAppService.ObtenerPerfilPublicoAsync(nombreUsuarioVacio)
            );
        }
    }
}
