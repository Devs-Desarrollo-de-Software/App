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

namespace TurisGo.Experiencias
{
    public class ExperienciaAppService_Tests<TStartupModule> : TurisGoApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IRepository<Experiencia, Guid> _experienciaRepository;
        private readonly IRepository<Destino, Guid> _destinoRepository;
        private readonly IExperienciaAppService _service;

        public ExperienciaAppService_Tests()
        {
            _experienciaRepository = GetRequiredService<IRepository<Experiencia, Guid>>();
            _destinoRepository = GetRequiredService<IRepository<Destino, Guid>>();
            _service = GetRequiredService<IExperienciaAppService>();
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







    }
}
