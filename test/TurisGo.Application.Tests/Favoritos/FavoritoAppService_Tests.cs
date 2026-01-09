using Autofac.Core;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisGo.Destinos;
using TurisGo.Experiencias;
using TurisGo.Usuarios;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.Users;
using Xunit;

namespace TurisGo.Favoritos
{
    public abstract class FavoritoAppService_Tests<TStartupModule> : TurisGoApplicationTestBase<TStartupModule>
      where TStartupModule : IAbpModule
    {
        private readonly IFavoritoAppService _service;
        private readonly IRepository<Favorito, Guid> _favoritoRepository;
        private readonly IRepository<Destino, Guid> _destinoRepository;
        private readonly ICurrentUser _currentUser;

        public FavoritoAppService_Tests()
        {
            _service = GetRequiredService<IFavoritoAppService>();
            _favoritoRepository = GetRequiredService<IRepository<Favorito, Guid>>();
            _destinoRepository = GetRequiredService<IRepository<Destino, Guid>>();
            _currentUser = GetRequiredService<ICurrentUser>();
        }

        // Helper method to create a test Destino
        private async Task<Destino> CreateTestDestinoAsync(string nombre = "Test Destino")
        {
            var destino = new Destino(
                Guid.NewGuid(),
                nombre,
                "Argentina",
                1000000,
                "https://test.com/image.jpg",
                new Coordenada(-34.6037, -58.3816)
            );

            return await _destinoRepository.InsertAsync(destino, autoSave: true);
        }


        // 6.1. Agregar destino a lista de favoritos

        [Fact]
        public async Task AgregarFavoritoAsync_Should_Add_Favorito_Successfully()
        {
            // Arrange
            var destino = await CreateTestDestinoAsync();
            var input = new CrearFavoritoDto
            {
                DestinoId = destino.Id
            };

            // Act
            var result = await _service.AgregarFavoritoAsync(input);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(Guid.Empty);
            result.DestinoId.ShouldBe(destino.Id);
            result.UserId.ShouldNotBe(Guid.Empty);

            // Verrificar que se guardo en la base de datos
            var favoritoEnDb = await _favoritoRepository.FindAsync(result.Id);
            favoritoEnDb.ShouldNotBeNull();
            favoritoEnDb.DestinoId.ShouldBe(destino.Id);
        }

        [Fact]
        public async Task AgregarFavoritoAsync_Should_Throw_Exception_When_Already_Exists()
        {
            // Arrange
            var destino = await CreateTestDestinoAsync("Londres");
            var input = new CrearFavoritoDto
            {
                DestinoId = destino.Id
            };

            // Agregar el favorito por primera vez
            await _service.AgregarFavoritoAsync(input);

            // Act & Assert
            var exception = await Should.ThrowAsync<UserFriendlyException>(
                async () => await _service.AgregarFavoritoAsync(input)
            );

            exception.Message.ShouldContain("ya está en tu lista de favoritos");
        }

        [Fact]
        public async Task AgregarFavoritoAsync_Should_Assign_Current_User_Id()
        {
            // Arrange
            var destino = await CreateTestDestinoAsync("Tokio");
            var input = new CrearFavoritoDto
            {
                DestinoId = destino.Id
            };

            // Act
            var result = await _service.AgregarFavoritoAsync(input);

            // Assert
            result.UserId.ShouldNotBe(Guid.Empty);

            // Verificar que el favorito pertenece al usuario actual del test
            var favoritoEnBd = await _favoritoRepository.GetAsync(result.Id);
            favoritoEnBd.UserId.ShouldBe(result.UserId);
        }

        [Fact]
        public async Task AgregarFavoritoAsync_Should_Throw_Exception_When_Destino_Not_Exists()
        {
            // Arrange
            var destinoIdInexistente = Guid.NewGuid();
            var input = new CrearFavoritoDto
            {
                DestinoId = destinoIdInexistente
            };

            // Act & Assert
            var exception = await Should.ThrowAsync<UserFriendlyException>(
                async () => await _service.AgregarFavoritoAsync(input)
            );

            exception.Message.ShouldContain("no existe");
        }

    }
}
