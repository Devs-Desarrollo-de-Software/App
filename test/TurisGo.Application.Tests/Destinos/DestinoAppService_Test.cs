using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;
using Xunit.Abstractions;

namespace TurisGo.Destinos
{
    public class DestinoAppService_Test : TurisGoApplicationTestBase<TurisGoApplicationTestModule>
    {
        private readonly IDestinoAppService _destinoAppService;

        public DestinoAppService_Test()
        {
            _destinoAppService = GetRequiredService<IDestinoAppService>();
        }

        [Fact]
        public async Task Should_Created_A_Valid_Destino()
        {
            //Arrange
            var input = new CreateUpdateDestinoDto
            {
                Nombre = "Paris",
                Pais = "Francia",
                Poblacion = 2200000,
                Imagen = "https://prueba.com/paris.png",
                Coordenada = new CoordenadaDto { Latitud = 48.3131 , Longitud = 2.34 }
            };

            //Act
            var result = await _destinoAppService.CreateAsync(input);

            //Assert
            result.Id.ShouldNotBe(Guid.Empty);
            result.Nombre.ShouldBe("Paris");
        }

        [Fact]
        public async Task Should_Not_Allow_Invalid_Poblacion()
        {
            //Arrange
            var input = new CreateUpdateDestinoDto
            {
                Nombre = "Ciudad Invalida",
                Pais = "ErrorLand",
                Poblacion = 0,
                Imagen = "https://prueba.com/ciudad.png",
                Coordenada = new CoordenadaDto { Latitud = 10, Longitud = 20 }

            };

            //Act & Assert
            await Assert.ThrowsAsync<AbpValidationException>(
                async () => await _destinoAppService.CreateAsync(input)
            );
        }

        [Fact]
        public async Task Should_Not_Allow_Invalid_Coordenadas()
        {
            //Arrange
            var input = new CreateUpdateDestinoDto
            {
                Nombre = "Ciudad con Error",
                Pais = "ErrorLand",
                Poblacion = 100000,
                Imagen = "https://prueba.com/ciudad.png",
                Coordenada = new CoordenadaDto { Latitud = 200, Longitud = 999 }
            };

            //Act & Assert
            await Assert.ThrowsAsync<AbpValidationException>(
                async () => await _destinoAppService.CreateAsync(input)
            );
            
        }

        [Fact]
        public async Task Should_Not_Allow_Duplicate_Destinos()
        {
            var input = new CreateUpdateDestinoDto
            {
                Nombre = "Roma",
                Pais = "Italia",
                Poblacion = 120000,
                Imagen = "https://prueba.com/roma.png",
                Coordenada = new CoordenadaDto { Latitud = 41.2, Longitud = 12.4}

            };

            await _destinoAppService.CreateAsync(input);

            await Assert.ThrowsAsync<BusinessException>(
                async () => await _destinoAppService.CreateAsync(input)
                );
        }
       
    }

    

}
