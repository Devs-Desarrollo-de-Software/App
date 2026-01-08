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

        public UsuarioAppService_Tests()
        {
            _usuarioAppService = GetRequiredService<IUsuarioAppService>();
            _usuarioRepository = GetRequiredService<Volo.Abp.Domain.Repositories.IRepository<Usuario, Guid>>();
            _identityUserManager = GetRequiredService<IdentityUserManager>();
            _identityUserRepository = GetRequiredService<IIdentityUserRepository>();
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
    }
}
