using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisGo.Destinos;
using TurisGo.Favoritos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
using Xunit;

namespace TurisGo.Notificaciones
{
    public abstract class NotificacionAppService_Tests<TStartupModule> : TurisGoApplicationTestBase<TStartupModule>
       where TStartupModule : IAbpModule
    {
        private readonly INotificacionAppService _notificacionAppService;
        private readonly IRepository<Notificacion, Guid> _notificacionRepository;
        private readonly IRepository<Destino, Guid> _destinos;
        private readonly IRepository<Favorito, Guid> _favoritoRepository;
        private readonly ICurrentUser _currentUser;

        protected NotificacionAppService_Tests()
        {
            _notificacionAppService = GetRequiredService<INotificacionAppService>();
            _notificacionRepository = GetRequiredService<IRepository<Notificacion, Guid>>();
            _destinos = GetRequiredService<IRepository<Destino, Guid>>();
            _favoritoRepository = GetRequiredService<IRepository<Favorito, Guid>>();
            _currentUser = GetRequiredService<ICurrentUser>();
        }

        #region Helper Methods

        /// <summary>
        /// Crea un destino de prueba
        /// AJUSTAR según el constructor de TU entidad Destino
        /// </summary>
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

            return await _destinos.InsertAsync(destino, autoSave: true);
        }

        /// <summary>
        /// Agrega un destino a favoritos del usuario actual
        /// </summary>
        private async Task<Favorito> AddToFavoritosAsync(Guid destinoId)
        {
            var userId = _currentUser.Id ?? throw new Exception("User not authenticated in test");

            var favorito = new Favorito(
                Guid.NewGuid(),  // Sin paréntesis
                userId,
                destinoId
            );

            await _favoritoRepository.InsertAsync(favorito);
            return favorito;
        }

        /// <summary>
        /// Crea un destino y lo agrega a favoritos en un solo paso
        /// </summary>
        private async Task<Destino> CreateDestinoConFavoritoAsync(string nombre = "Test City")
        {
            var destino = await CreateTestDestinoAsync(nombre);
            await AddToFavoritosAsync(destino.Id);
            return destino;
        }

        #endregion

        #region Admin Tests - Notificacion

        [Fact]
        public async Task Should_Notify_Using_Generic_Method()
        {
            // Arrange
            var destino = await CreateDestinoConFavoritoAsync("Londres");

            // Act
            var resultado = await _notificacionAppService.NotificarCambioDestinoAsync(
                destino.Id,
                "Cambio importante",
                "Se ha detectado un cambio significativo",
                TipoNotificacion.CambioDestino
            );

            // Assert
            resultado.UsuariosNotificados.ShouldBe(1);
            resultado.Tipo.ShouldBe(TipoNotificacion.CambioDestino);
        }

        #endregion

    }

}

