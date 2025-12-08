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

        // Helper method to create a test Experiencia
        private async Task<Experiencia> CreateTestExperienciaAsync(TipoValoracion valoracion, string? titulo, string? descripcion)
        {
            var destino = await CreateTestDestinoAsync();

            var experiencia = new Experiencia(
                Guid.NewGuid(),
                _currentUser.Id!.Value,
                destino.Id,
                titulo,
                descripcion,
                valoracion
            );

            return await _experienciaRepository.InsertAsync(experiencia, autoSave: true);
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

        [Fact]
        public async Task GetListExperienciaAsync_Should_Get_List_Of_Experiencia()
        {
            // Arrange
            var destino = await CreateTestDestinoAsync();
            var experiencia = new Experiencia(
                Guid.NewGuid(),
                Guid.NewGuid(),
                destino.Id,
                "Titulo",
                "Descripcion",
                TipoValoracion.Positiva
                );

            await _experienciaRepository.InsertAsync(experiencia, autoSave: true);

            // Act
            var result = await _service.GetListExperienciasAsync(destino.Id);

            // Assert
            result.ShouldNotBeNull();
            result.Experiencias.Count.ShouldBe(1);
            result.DestinoId.ShouldBe(destino.Id);
            result.Experiencias.ShouldNotBeNull();
        }

        [Fact]
        public async Task GetListExperienciasAsync_Should_Throw_Exception_When_Destino_Does_Not_Exist()
        {
            // Arrange
            var destinoId = Guid.NewGuid(); // ID de destino inexistente

            // Act & Assert
            await Should.ThrowAsync<BusinessException>(async () =>
            {
                await _service.GetListExperienciasAsync(destinoId);
            });
        }

        [Fact]
        public async Task GetListAsync_Should_Return_All_Experiencias_When_No_Filter()
        {
            // Arrange
            await CreateTestExperienciaAsync(TipoValoracion.Positiva, "Exp 1","Des 1");
            await CreateTestExperienciaAsync(TipoValoracion.Neutra, "Exp 2","Des 2");
            await CreateTestExperienciaAsync(TipoValoracion.Negativa, "Exp 3","Des 3");

            var input = new GetExperienciasListDto
            {
                Valoracion = null,
                MaxResultCount = 10,
                SkipCount = 0
            };

            // Act
            var result = await _service.GetListAsync(input);

            // Assert
            result.TotalCount.ShouldBeGreaterThanOrEqualTo(3);
            result.Items.Count.ShouldBeGreaterThanOrEqualTo(3);

            result.Items.ShouldContain(x => x.Valoracion == TipoValoracion.Positiva);
            result.Items.ShouldContain(x => x.Valoracion == TipoValoracion.Neutra);
            result.Items.ShouldContain(x => x.Valoracion == TipoValoracion.Negativa);

        }

        [Fact]
        public async Task GetListAsync_Should_Return_Only_Positivas_When_Filtered()
        {
            // Arrange
            await CreateTestExperienciaAsync(TipoValoracion.Positiva,"Exp 1", "Des 1");
            await CreateTestExperienciaAsync(TipoValoracion.Positiva, "Exp 2", "Des 2");
            await CreateTestExperienciaAsync(TipoValoracion.Neutra, "Exp 3", "Des 3");
            await CreateTestExperienciaAsync(TipoValoracion.Negativa, "Exp 4", "Des 4");

            var input = new GetExperienciasListDto
            {
                Valoracion = TipoValoracion.Positiva, 
                SkipCount = 0,
                MaxResultCount = 10
            };

            // Act
            var result = await _service.GetListAsync(input);

            // Assert
            result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
            result.Items.ShouldAllBe(x => x.Valoracion == TipoValoracion.Positiva);

            // Verificar que NO hay otros tipos
            result.Items.ShouldNotContain(x => x.Valoracion == TipoValoracion.Neutra);
            result.Items.ShouldNotContain(x => x.Valoracion == TipoValoracion.Negativa);
        }

        [Fact]
        public async Task GetListAsync_Should_Only_Return_Current_User_Experiencias()
        {
            // Arrange
            await CreateTestExperienciaAsync(TipoValoracion.Positiva, "Mi exp","Mi desc");

            // Crear experiencia de otro usuario
            var destino = await CreateTestDestinoAsync();
            var expAjena = new Experiencia(
                Guid.NewGuid(),
                Guid.NewGuid(),  // Otro usuario
                destino.Id,
                "Experiencia ajena",
                "No debería aparecer",
                TipoValoracion.Positiva
            );
            await _experienciaRepository.InsertAsync(expAjena, autoSave: true);

            var input = new GetExperienciasListDto
            {
                Valoracion = TipoValoracion.Positiva,
                SkipCount = 0,
                MaxResultCount = 10
            };

            // Act
            var result = await _service.GetListAsync(input);

            // Assert
            result.Items.ShouldAllBe(x => x.UserId == _currentUser.Id!.Value);
            result.Items.ShouldNotContain(x => x.Titulo == "Experiencia ajena");
        }

        [Fact]
        public async Task GetListAsync_Should_Find_Experiencias_By_Keyword_In_Titulo()
        {
            // Arrange
            await CreateTestExperienciaAsync(
                TipoValoracion.Positiva,
                "Excelente gastronomía local",
                "Probé platos típicos"  
            );
            await CreateTestExperienciaAsync(
                TipoValoracion.Positiva,
                "Hermosas playas",
                "Las playas son increíbles"
            );
            
            var input = new GetExperienciasListDto
            {
                PalabraClave = "gastronomía",
                SkipCount = 0,
                MaxResultCount = 10
            };

            // Act
            var result = await _service.GetListAsync(input);

            // Assert
            result.TotalCount.ShouldBeGreaterThanOrEqualTo(1);
            result.Items.ShouldContain(x => x.Titulo.Contains("gastronomía", StringComparison.OrdinalIgnoreCase));
            result.Items.ShouldNotContain(x => x.Titulo.Contains("playas", StringComparison.OrdinalIgnoreCase));

        }

        [Fact]
        public async Task GetListAsync_Should_Find_Experiencias_By_Keyword_In_Descripcion()
        {
            // Arrange
            await CreateTestExperienciaAsync(
                TipoValoracion.Positiva,
                "Título genérico",
                "La seguridad del lugar es excelente"    
            );
            await CreateTestExperienciaAsync(
                TipoValoracion.Positiva,
                "Otro título",
                "Buena experiencia en general"
            );

            var input = new GetExperienciasListDto
            {
                PalabraClave = "seguridad",
                SkipCount = 0,
                MaxResultCount = 10
            };

            // Act
            var result = await _service.GetListAsync(input);

            // Assert
            result.TotalCount.ShouldBeGreaterThanOrEqualTo(1);
            result.Items.ShouldContain(x => x.Descripcion.Contains("seguridad", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task GetListAsync_Should_Return_Empty_When_No_Match()
        {
            // Arrange
            await CreateTestExperienciaAsync(
                TipoValoracion.Positiva,
                "Experiencia urbana",
                "Ciudad muy activa"
            );

            var input = new GetExperienciasListDto
            {
                PalabraClave = "montaña",  // No hay coincidencias
                SkipCount = 0,
                MaxResultCount = 10
            };

            // Act
            var result = await _service.GetListAsync(input);

            // Assert
            result.Items.ShouldBeEmpty();
            result.TotalCount.ShouldBe(0);
        }




    }
}
