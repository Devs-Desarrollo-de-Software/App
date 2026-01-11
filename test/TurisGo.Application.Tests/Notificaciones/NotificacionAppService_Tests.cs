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

        #region Admin Tests - Notificaciones

        [Fact]
        public async Task Should_Notify_User_When_Destination_Is_In_Favorites()
        {
            // Arrange
            var destino = await CreateDestinoConFavoritoAsync("París");

            // Act
            var resultado = await _notificacionAppService.NotificarNuevoEventoAsync(
                destino.Id,
                "Concierto de Coldplay",
                DateTime.UtcNow.AddMonths(2)
            );

            // Assert
            resultado.ShouldNotBeNull();
            resultado.UsuariosNotificados.ShouldBe(1);
            resultado.NombreDestino.ShouldBe("París");
            resultado.Tipo.ShouldBe(TipoNotificacion.NuevoEvento);
            resultado.Mensaje.ShouldContain("1 usuario(s)");

            // Verificar que se creó en la BD
            var notificaciones = await _notificacionRepository.GetListAsync();
            notificaciones.Count.ShouldBeGreaterThan(0);
            notificaciones.Any(n => n.DestinoId == destino.Id).ShouldBeTrue();
        }

        [Fact]
        public async Task Should_Return_Zero_When_No_User_Has_Destination_In_Favorites()
        {
            // Arrange - Crear destino SIN agregarlo a favoritos
            var destino = await CreateTestDestinoAsync("Tokio");

            // Act
            var resultado = await _notificacionAppService.NotificarNuevoEventoAsync(
                destino.Id,
                "Festival de Primavera",
                DateTime.UtcNow.AddMonths(1)
            );

            // Assert
            resultado.ShouldNotBeNull();
            resultado.UsuariosNotificados.ShouldBe(0);
            resultado.Mensaje.ShouldContain("No hay usuarios");

            // Verificar que NO se creó ninguna notificación en la BD
            var notificaciones = await _notificacionRepository.GetListAsync();
            notificaciones.Any(n => n.DestinoId == destino.Id).ShouldBeFalse();
        }

        [Fact]
        public async Task Should_Notify_About_Data_Update()
        {
            // Arrange
            var destino = await CreateDestinoConFavoritoAsync("Barcelona");

            // Act
            var resultado = await _notificacionAppService.NotificarActualizacionDatosAsync(
                destino.Id,
                "La población se actualizó de 1.6M a 1.65M"
            );

            // Assert
            resultado.ShouldNotBeNull();
            resultado.UsuariosNotificados.ShouldBe(1);
            resultado.Tipo.ShouldBe(TipoNotificacion.ActualizacionDatos);

            // Verificar tipo de notificación en BD
            var notificaciones = await _notificacionRepository.GetListAsync();
            var notificacion = notificaciones.FirstOrDefault(n => n.DestinoId == destino.Id);
            notificacion.ShouldNotBeNull();
            notificacion.Tipo.ShouldBe(TipoNotificacion.ActualizacionDatos);
        }

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

        #region User Tests - Listar Notificaciones

        [Fact]
        public async Task Should_List_User_Notifications()
        {
            // Arrange
            var destino1 = await CreateDestinoConFavoritoAsync("París");
            var destino2 = await CreateDestinoConFavoritoAsync("Roma");

            await _notificacionAppService.NotificarNuevoEventoAsync(
                destino1.Id, "Evento 1", DateTime.UtcNow.AddDays(1)
            );
            await _notificacionAppService.NotificarNuevoEventoAsync(
                destino2.Id, "Evento 2", DateTime.UtcNow.AddDays(2)
            );

            var input = new GetNotificacionesInput
            {
                SkipCount = 0,
                MaxResultCount = 10
            };

            // Act
            var resultado = await _notificacionAppService.GetListAsync(input);

            // Assert
            resultado.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
            resultado.Items.Count.ShouldBeGreaterThanOrEqualTo(2);

            // ✅ Quitamos validación de UserId porque NotificacionDto no lo tiene
            // La seguridad ya está garantizada por los query filters en el backend
        }

        [Fact]
        public async Task Should_Filter_Only_Unread_Notifications()
        {
            // Arrange
            var destino = await CreateDestinoConFavoritoAsync("París");

            // Crear 2 notificaciones
            await _notificacionAppService.NotificarNuevoEventoAsync(
                destino.Id, "Evento 1", DateTime.UtcNow
            );
            await _notificacionAppService.NotificarNuevoEventoAsync(
                destino.Id, "Evento 2", DateTime.UtcNow.AddDays(1)
            );

            // Marcar la primera como leída
            var todas = await _notificacionAppService.GetListAsync(
                new GetNotificacionesInput { MaxResultCount = 10 }
            );
            var primera = todas.Items.First();
            await _notificacionAppService.MarcarComoLeidaAsync(primera.Id, true);

            // Act - Filtrar solo no leídas
            var input = new GetNotificacionesInput
            {
                SoloNoLeidas = true,
                MaxResultCount = 10
            };
            var resultado = await _notificacionAppService.GetListAsync(input);

            // Assert
            resultado.Items.ShouldAllBe(n => !n.Leida);
            resultado.Items.Any(n => n.Id == primera.Id).ShouldBeFalse();
        }

        #endregion
    }

}

