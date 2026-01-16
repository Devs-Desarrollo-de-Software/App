using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Volo.Abp.Validation;
using Xunit;

namespace TurisGo.Destinos
{
    public class CityAppService_Tests : TurisGoApplicationTestBase<TurisGoApplicationModule>
    {

        private readonly DestinoAppService _destinoAppService;
        private readonly Mock<ICitySearchService> _mockCityService;

        public CityAppService_Tests()
        {
            _mockCityService = new Mock<ICitySearchService>();

            var mockRepo = new Mock<IRepository<Destino, Guid>>();

            _destinoAppService = new DestinoAppService(mockRepo.Object, _mockCityService.Object);

        }


        // ----------------- Pruebas para el metodo SearchCitiesByName ------------------------

        [Fact]
        public async Task SearchByName_Should_Return_CityList()
        {
            //Arrange
            var cities = new List<CityDto>
             {
                 new CityDto
                 {
                     Name = "Roma",
                     Country = "Italia",
                     Population = 2873000,
                     Latitude = 41.9,
                     Longitude = 12.5
                 },

                 new CityDto
                 {
                     Name = "Romang",
                     Country = "Argentina",
                     Population = 12000,
                     Latitude = -29.5,
                     Longitude = -59.7
                 }
             };


            _mockCityService
                .Setup(x => x.SearchCitiesByNameAsync("Roma"))
                .ReturnsAsync(cities);

            //Act
            var result = await _destinoAppService.BuscarCiudadesPorNombreAsync("Roma");

            //Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result[0].Name.ShouldBe("Roma");
            result[1].Country.ShouldBe("Argentina");
        }


        [Fact]
        public async Task SearchByName_Should_Throw_Exception_When_NameIsEmpty()
        {
            //Act & Assert
            await Should.ThrowAsync<AbpValidationException>(async () =>
            {
                await _destinoAppService.BuscarCiudadesPorNombreAsync("");
            });
        }


        [Fact]
        public async Task SearchByName_Should_Return_EmptyList_When_NoResults()
        {
            //Arrange
            _mockCityService
                .Setup(x => x.SearchCitiesByNameAsync("Ciudadinventada"))
                .ReturnsAsync(new List<CityDto>());

            //Act
            var result = await _destinoAppService.BuscarCiudadesPorNombreAsync("CiudadInventada");

            //Assert
            result.ShouldNotBeNull();

        }

        [Fact]
        public async Task SearchByName_Should_Normalize_Input_CaseInsensitive()
        {
            //Arrange
            var cities = new List<CityDto>
            {
                new CityDto
                {
                    Name = "Colon",
                    Country = "Argentina",
                    Population = 24000,
                    Latitude = -32.22,
                    Longitude = -61.09
                }
            };

            // El servicio debe recibir "Colon" (normalizado)
            _mockCityService
                .Setup(x => x.SearchCitiesByNameAsync("Colon"))
                .ReturnsAsync(cities);

            //Act - Enviamos "CoLoN" pero debe normalizarse a "Colon"
            var result = await _destinoAppService.BuscarCiudadesPorNombreAsync("CoLoN");

            //Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].Name.ShouldBe("Colon");

            // Verificamos que se llamó con el texto normalizado
            _mockCityService.Verify(
                x => x.SearchCitiesByNameAsync("Colon"),
                Times.Once
            );
        }

        [Fact]
        public async Task SearchByName_Should_Throw_Exception_When_ApiFails()
        {
            //Arrange
            _mockCityService
                .Setup(x => x.SearchCitiesByNameAsync("Roma"))
                .ThrowsAsync(new HttpRequestException("Error en la API externa"));

            //Act & Assert
            await Should.ThrowAsync<HttpRequestException>(async () =>
            {
                await _destinoAppService.BuscarCiudadesPorNombreAsync("Roma");
            });

        }

        // --------------- Pruebas para el metodo FilterCities() --------------------------

        [Fact]
        public async Task Filter_Should_Return_EmptyList_When_No_Filters_Provided()
        {
            // Arrange
            _mockCityService
                .Setup(x => x.FilterCitiesAsync(null, 0, null, null))
                .ReturnsAsync(new List<CityDto>());

            // Act
            var result = await _destinoAppService.FiltrarCiudadesAsync(null, 0, null);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();

        }

        [Fact]
        public async Task Filter_Should_Throw_When_Population_Is_Negative()
        {
            await Should.ThrowAsync<AbpValidationException>(() =>
            
                 _destinoAppService.FiltrarCiudadesAsync("AR", -1, null)
            );
        }

        [Fact]
        public async Task Filter_Should_Call_ICitySearchService_With_Correct_Parameters()
        {
            // Arrange - Espera "Ar" normalizado (primera mayúscula, resto minúscula)
            _mockCityService
                .Setup(x => x.FilterCitiesAsync("Ar", 1000000, null, null))
                .ReturnsAsync(new List<CityDto>());

            // Act
            await _destinoAppService.FiltrarCiudadesAsync("AR", 1000000, null);

            // Assert
            _mockCityService.Verify(
                x => x.FilterCitiesAsync("Ar", 1000000, null, null),
                Times.Once
                );
        }

        [Fact]
        public async Task Filter_Should_Return_Empty_List_When_No_Result()
        {
            // Arrange - Espera "Xx" normalizado
            _mockCityService
                .Setup(x => x.FilterCitiesAsync("Xx", 0, null, null))
                .ReturnsAsync(new List<CityDto>());

            // Act
            var result = await _destinoAppService.FiltrarCiudadesAsync("XX", 0, null);

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Filter_Should_Throw_Api_Fails()
        {
            // Arrange - Espera "Ar" normalizado
            _mockCityService
                .Setup(x => x.FilterCitiesAsync("Ar", 1000000, null, null))
                .ThrowsAsync(new HttpRequestException("API error"));

            // Act & Assert
            await Should.ThrowAsync<HttpRequestException>(() =>

                _destinoAppService.FiltrarCiudadesAsync("AR", 1000000, null)
            );
        }

        [Fact]
        public async Task Filter_Should_Work_With_Only_Region()
        {
            var cities = new List<CityDto>
            {
                new CityDto { Name = "Cordoba", Country = "Argentina", Population = 1300000 }
            };

            _mockCityService
                .Setup(x => x.FilterCitiesAsync(null, 0, "Cor", null))
                .ReturnsAsync(cities);

            var result = await _destinoAppService.FiltrarCiudadesAsync(null, 0, "cor");

            result.ShouldNotBeEmpty();

        }

        [Fact]
        public async Task Filter_Should_Normalize_Country_CaseInsensitive()
        {
            // Arrange
            var cities = new List<CityDto>
            {
                new CityDto { Name = "Buenos Aires", Country = "Argentina", Population = 2890000 }
            };

            // Mock espera "Argentina" normalizado
            _mockCityService
                .Setup(x => x.FilterCitiesAsync("Argentina", 0, null, null))
                .ReturnsAsync(cities);

            // Act - Enviamos "ARGENTINA" en mayúsculas
            var result = await _destinoAppService.FiltrarCiudadesAsync("ARGENTINA", 0, null);

            // Assert
            result.ShouldNotBeEmpty();
            _mockCityService.Verify(
                x => x.FilterCitiesAsync("Argentina", 0, null, null),
                Times.Once
            );
        }

        [Fact]
        public async Task Filter_Should_Normalize_Region_CaseInsensitive()
        {
            // Arrange
            var cities = new List<CityDto>
            {
                new CityDto { Name = "Colon", Country = "Argentina", Population = 24000 }
            };

            _mockCityService
                .Setup(x => x.FilterCitiesAsync("Argentina", 0, "Entre rios", null))
                .ReturnsAsync(cities);

            // Act - Enviamos "ENTRE RIOS" en mayúsculas
            var result = await _destinoAppService.FiltrarCiudadesAsync("Argentina", 0, "ENTRE RIOS");

            // Assert
            result.ShouldNotBeEmpty();
            _mockCityService.Verify(
                x => x.FilterCitiesAsync("Argentina", 0, "Entre rios", null),
                Times.Once
            );
        }

        [Fact]
        public async Task Filter_Should_Normalize_CityName_CaseInsensitive()
        {
            // Arrange
            var cities = new List<CityDto>
            {
                new CityDto { Name = "Cordoba", Country = "Argentina", Population = 1300000 }
            };

            _mockCityService
                .Setup(x => x.FilterCitiesAsync(null, 0, null, "Cordoba"))
                .ReturnsAsync(cities);

            // Act - Enviamos "cOrDoBa" mezclado
            var result = await _destinoAppService.FiltrarCiudadesAsync(null, 0, null, "cOrDoBa");

            // Assert
            result.ShouldNotBeEmpty();
            _mockCityService.Verify(
                x => x.FilterCitiesAsync(null, 0, null, "Cordoba"),
                Times.Once
            );
        }

        // ---------------------------- Pruebas para el metodo GetDetailsAsync --------------------------

        [Fact]
        public async Task GetCityDetails_Should_Return_Complete_City_Information()
        {
            // Arrange
            var cityDetail = new CityDetailDto
            {
                Id = 3435910,
                Name = "Buenos Aires",
                Country = "Argentina",
                CountryCode = "AR",
                Region = "Buenos Aires F.D.",
                RegionCode = "C",
                Latitude = -34.61315,
                Longitude = -58.37723,
                Population = 2890151,
                WikiDataId = "Q1486",
                TimeZone = "America/Argentina/Buenos_Aires",
                ElevationMeters = 25,
                Type = "CITY"
            };

            _mockCityService
                .Setup(x => x.GetCityDetailsAsync(3435910))
                .ReturnsAsync(cityDetail);

            // Act
            var result = await _destinoAppService.ObtenerDetalleCiudadAsync(3435910);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(3435910);
            result.Name.ShouldBe("Buenos Aires");
            result.Country.ShouldBe("Argentina");
            result.CountryCode.ShouldBe("AR");
            result.Region.ShouldBe("Buenos Aires F.D.");
            result.RegionCode.ShouldBe("C");
            result.Latitude.ShouldBe(-34.61315);
            result.Longitude.ShouldBe(-58.37723);
            result.Population.ShouldBe(2890151);
            result.WikiDataId.ShouldBe("Q1486");
            result.TimeZone.ShouldBe("America/Argentina/Buenos_Aires");
            result.ElevationMeters.ShouldBe(25);
            result.Type.ShouldBe("CITY");
        }

        [Fact]
        public async Task GetCityDetails_Should_Throw_When_CityId_Is_Zero()
        {
            // Act & Assert
            await Should.ThrowAsync<AbpValidationException>(async () =>
            {
                await _destinoAppService.ObtenerDetalleCiudadAsync(0);
            });
        }

        [Fact]
        public async Task GetCityDetails_Should_Throw_When_City_Is_Negative()
        {
            // Act & Assert
            await Should.ThrowAsync<AbpValidationException>(async () =>
            {
                await _destinoAppService.ObtenerDetalleCiudadAsync(-1);
            });
        }

        [Fact]
        public async Task GetCityDetails_Should_Throw_EntityNotFoundException_When_City_Not_Found()
        {
            // Arrange
            _mockCityService
                .Setup(x => x.GetCityDetailsAsync(999999999))
                .ThrowsAsync(new EntityNotFoundException(typeof(CityDetailDto), 999999999));

            // Act & Assert
            await Should.ThrowAsync<EntityNotFoundException>(async () =>
            {
                await _destinoAppService.ObtenerDetalleCiudadAsync(999999999);
            });
        }

        [Fact]
        public async Task GetCityDetails_Should_Call_Service_With_Correct_CityId()
        {
            // Arrange
            var cityDetail = new CityDetailDto
            {
                Id = 12345,
                Name = "Test City",
                Country = "Test Country",
                CountryCode = "TC",
                Latitude = 0,
                Longitude = 0,
                Population = 100000
            };

            _mockCityService
                .Setup(x => x.GetCityDetailsAsync(12345))
                .ReturnsAsync(cityDetail);

            // Act
            await _destinoAppService.ObtenerDetalleCiudadAsync(12345);

            // Assert
            _mockCityService.Verify(
                x => x.GetCityDetailsAsync(12345),
                Times.Once
                );
        }

        [Fact]
        public async Task GetCityDetails_Should_Throw_When_Api_Fails()
        {
            // Arrange
            _mockCityService
                .Setup(x => x.GetCityDetailsAsync(12345))
                .ThrowsAsync(new HttpRequestException("Error en la API externa"));

            // Act & Assert
            await Should.ThrowAsync<HttpRequestException>(async () =>
            {
                await _destinoAppService.ObtenerDetalleCiudadAsync(12345);
            });
        }

        [Fact]
        public async Task GetCityDetails_Should_Include_All_Optional_Fields()
        {
            // Arrange
            var cityDetail = new CityDetailDto
            {
                Id = 54321,
                Name = "Roma",
                Country = "Italy",
                CountryCode = "IT",
                Region = "Lazio",
                RegionCode = "LAZ",
                Latitude = 41.9,
                Longitude = 12.5,
                Population = 2873000,
                WikiDataId = "Q220",
                TimeZone = "Europe/Rome",
                ElevationMeters = 21,
                Type = "CITY"
            };

            _mockCityService
                .Setup(x => x.GetCityDetailsAsync(54321))
                .ReturnsAsync(cityDetail);

            // Act
            var result = await _destinoAppService.ObtenerDetalleCiudadAsync(54321);

            // Assert
            result.WikiDataId.ShouldNotBeNullOrWhiteSpace();
            result.TimeZone.ShouldNotBeNullOrWhiteSpace();
            result.ElevationMeters.ShouldNotBeNull();
            result.Type.ShouldBe("CITY");
        }



    }
    
}
