using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisGo.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using Volo.Abp.Validation;
using Xunit;

namespace TurisGo.Destinos
{
    public abstract class DestinoAppService_Tests<TStartupModule> : TurisGoApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IDestinoAppService _service;
        private readonly IDbContextProvider<TurisGoDbContext> _DbContextProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        protected DestinoAppService_Tests()
        {
            _service = GetRequiredService<IDestinoAppService>();
            _DbContextProvider = GetRequiredService<IDbContextProvider<TurisGoDbContext>>();
            _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedDestinoDto()
        {
            //Arrange
            var input = new CreateUpdateDestinoDto
            {
                Nombre = "Paris",
                Pais = "Francia",
                Poblacion = 101010,
                Imagen = "https://prueba.com/paris.png",
                Coordenada = new CoordenadaDto { Latitud = 11.2, Longitud = 24.2 }
            };

            //Act
            var result = await _service.CreateAsync(input);

            //Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(Guid.Empty);
            result.Nombre.ShouldBe(input.Nombre);
            result.Pais.ShouldBe(input.Pais);
            result.Poblacion.ShouldBe(input.Poblacion);
            result.Imagen.ShouldBe(input.Imagen);
        }

        [Fact]
        public async Task CreateAsync_ShouldPersistDestinoInDatabase()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                //Arrange
                var input = new CreateUpdateDestinoDto
                {
                    Nombre = "Tokio",
                    Pais = "Japon",
                    Poblacion = 125000,
                    Imagen = "https://prueba.com/tokio.png",
                    Coordenada = new CoordenadaDto { Latitud = 12, Longitud = 12.1 }
                };

                //Act
                var result = await _service.CreateAsync(input);

                //Assert
                var dBContext = await _DbContextProvider.GetDbContextAsync();
                var savedDestino = await dBContext.Destinos.FindAsync(result.Id);

                savedDestino.ShouldNotBeNull();
                savedDestino.Nombre.ShouldBe(input.Nombre);
                savedDestino.Pais.ShouldBe(input.Pais);
                savedDestino.Poblacion.ShouldBe(input.Poblacion);
                savedDestino.Imagen.ShouldBe(input.Imagen);
                savedDestino.Coordenada.Latitud.ShouldBe(input.Coordenada.Latitud);
                savedDestino.Coordenada.Longitud.ShouldBe(input.Coordenada.Longitud);
            }

        }


        [Fact]
        public async Task Should_Not_Allow_Invalid_Coordenadas()
        {
            //Arrange
            var input = new CreateUpdateDestinoDto
            {
                Nombre = "Error",
                Pais = "ErrorLand",
                Poblacion = 10000,
                Imagen = "https://prueba.com/error.png",
                Coordenada = new CoordenadaDto { Latitud = 200, Longitud = 999 }
            };

            //Act & Assert
            await Should.ThrowAsync<AbpValidationException>(async () =>
            {
                await _service.CreateAsync(input);
            });
        }

        [Fact]
        public async Task Should_Not_Allow_Invalid_Country_Name()
        {
            var input = new CreateUpdateDestinoDto
            {
                Nombre = "Sevilla",
                Pais = "Esp@ña", // caracter inválido
                Poblacion = 700000,
                Imagen = "https://prueba.com/sevilla.png",
                Coordenada = new CoordenadaDto { Latitud = 37.39, Longitud = -5.99 }
            };

            await Should.ThrowAsync<AbpValidationException>(async () =>
            {
                await _service.CreateAsync(input);
            });
        }

        [Fact]
        public async Task Should_Not_Allow_Invalid_Image_Url()
        {
            var input = new CreateUpdateDestinoDto
            {
                Nombre = "Lisboa",
                Pais = "Portugal",
                Poblacion = 600000,
                Imagen = "imagen_sin_url", // inválido
                Coordenada = new CoordenadaDto { Latitud = 38.72, Longitud = -9.13 }
            };

            await Should.ThrowAsync<AbpValidationException>(async () =>
            {
                await _service.CreateAsync(input);
            });
        }

        [Fact]
        public async Task Should_Not_Allow_Zero_Population()
        {
            var input = new CreateUpdateDestinoDto
            {
                Nombre = "Ciudad Ficticia",
                Pais = "Nowhere",
                Poblacion = 0,
                Imagen = "https://prueba.com/nowhere.png",
                Coordenada = new CoordenadaDto { Latitud = 10, Longitud = 20 }
            };

            await Should.ThrowAsync<AbpValidationException>(async () =>
            {
                await _service.CreateAsync(input);
            });
        }



    }
}
