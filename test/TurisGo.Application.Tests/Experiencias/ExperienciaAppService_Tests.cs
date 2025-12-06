using Volo.Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisGo.Destinos;
using Volo.Abp.Modularity;
using Xunit;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Users;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Authorization;

namespace TurisGo.Experiencias
{
    public abstract class ExperienciaAppService_Tests<TStartupModule> : TurisGoApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IRepository<Experiencia, Guid> _experienciaRepository;
        private readonly IRepository<Destino, Guid> _destinoRepository;
        private readonly IExperienciaAppService _service;
        private readonly ICurrentUser _currentUser;

        public ExperienciaAppService_Tests()
        {
            _experienciaRepository = GetRequiredService<IRepository<Experiencia, Guid>>();
            _destinoRepository = GetRequiredService<IRepository<Destino, Guid>>();
            _service = GetRequiredService<IExperienciaAppService>();
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

        [Fact]
        public async Task CreateAsync_Should_Create_Experiencia_When_Data_Is_Valid()
        {
            // Arrange
            var destino = await CreateTestDestinoAsync();
            var experiencia = new CreateExperienciaDto
            {
                DestinoId = destino.Id,
                Titulo = "Una experiencia inolvidable",
                Descripcion = "Disfruté mucho de mi visita a este destino.",
                Valoracion = TipoValoracion.Positiva
            };

            // Act
            var result = await _service.CreateAsync(experiencia);

            // Assert
            result.ShouldNotBeNull();
            result.DestinoId.ShouldBe(destino.Id);
            result.Titulo.ShouldBe(experiencia.Titulo);
            result.Descripcion.ShouldBe(experiencia.Descripcion);
            result.Valoracion.ShouldBe(experiencia.Valoracion);
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_Exception_When_Destino_Does_Not_Exist()
        {
            // Arrange
            var destinoId = Guid.NewGuid(); // ID de destino inexistente
            var experiencia = new CreateExperienciaDto
            {
                DestinoId = destinoId,
                Titulo = "Una experiencia inolvidable",
                Descripcion = "Disfruté mucho de mi visita a este destino.",
                Valoracion = TipoValoracion.Positiva
            };

            // Act & Assert
            await Should.ThrowAsync<BusinessException>(async () =>
                {
                    await _service.CreateAsync(experiencia);
                });

        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Experiencia_When_Data_Is_Valid()
        {
            // Arrange
            var destino = await CreateTestDestinoAsync();
            var userId = _currentUser.Id!.Value;
            var experiencia = new Experiencia(
                Guid.NewGuid(),
                userId,
                destino.Id,
                "Titulo Original",
                "Descripcion Original",
                TipoValoracion.Neutra
            );

            await _experienciaRepository.InsertAsync(experiencia, autoSave: true);

            var experienciaActualizada = new UpdateExperienciaDto
            {
                Titulo = "Titulo Actualizado",
                Descripcion = "Descripcion Actualizada",
                Valoracion = TipoValoracion.Positiva
            };

            // Act
            var result = await _service.UpdateAsync(experiencia.Id, experienciaActualizada);

            // Assert
            result.ShouldNotBeNull();
            result.DestinoId.ShouldBe(destino.Id);
            result.Titulo.ShouldBe(experienciaActualizada.Titulo);
            result.Descripcion.ShouldBe(experienciaActualizada.Descripcion);
            result.Valoracion.ShouldBe(experienciaActualizada.Valoracion);
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_Exception_When_Experiencia_Does_Not_Exist()
        {
            // Arrange
            var experienciaId = Guid.NewGuid(); // ID de experiencia inexistente   
            var experienciaActualizada = new UpdateExperienciaDto
            {
                Titulo = "Titulo Actualizado",
                Descripcion = "Descripcion Actualizada",
                Valoracion = TipoValoracion.Positiva
            };

            // Act & Assert
            await Should.ThrowAsync<EntityNotFoundException>(async () =>
            {
                await _service.UpdateAsync(experienciaId, experienciaActualizada);
            });

        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_Exception_When_User_Is_Not_Onwer()
        {
            // Arrange
            var destino = await CreateTestDestinoAsync();
            var userId = _currentUser.Id!.Value; // Simular actual

            // Experiencia creada por otro usuario
            var experiencia = new Experiencia(
                Guid.NewGuid(),
                Guid.NewGuid(), // Usuario diferente
                destino.Id,
                "Titulo Original",
                "Descripcion Original",
                TipoValoracion.Neutra
            );

            await _experienciaRepository.InsertAsync(experiencia, autoSave: true);

            // Datos para actualizar
            var experienciaActualizada = new UpdateExperienciaDto
            {
                Titulo = "Titulo Actualizado",
                Descripcion = "Descripcion Actualizada",
                Valoracion = TipoValoracion.Positiva
            };

            // Act & Assert
            await Should.ThrowAsync<AbpAuthorizationException>(async () =>
            {
                await _service.UpdateAsync(experiencia.Id, experienciaActualizada);
            });
        }

        [Fact]
        public async Task DeleteAsync_Should_Delete_Experiencia_When_User_Is_Owner()
        {
            // Arrange
            var destino = await CreateTestDestinoAsync();
            var userId = _currentUser.Id!.Value;
            var experiencia = new Experiencia(
                Guid.NewGuid(),
                userId,
                destino.Id,
                "Titulo a eliminar",
                "Descripcion a eliminar",
                TipoValoracion.Neutra
            );
            await _experienciaRepository.InsertAsync(experiencia, autoSave: true);

            // Act
            await _service.DeleteAsync(experiencia.Id);

            // Assert
            var experienciaInDb = await _experienciaRepository.FindAsync(experiencia.Id);
            experienciaInDb.ShouldBeNull();
        }

        
    }
}
