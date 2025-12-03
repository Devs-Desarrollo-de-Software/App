using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Xunit;

namespace TurisGo.Destinos
{
    [Collection("GeoDbIntegrationTests")]
    public abstract class CityAppService_Integration_Tests<TStartupModule> : TurisGoApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly ICitySearchService _service;


        protected CityAppService_Integration_Tests()
        {
            _service = GetRequiredService<ICitySearchService>();
        }

        // ---------------------- Pruebas para el metodo SearchByName ---------------------

        [Fact]
        public async Task SearchByName_Should_Return_Cities_Whem_Valid_Name()
        {
            
            // Act
            var result = await _service.SearchCitiesByNameAsync("Buenos Aires");

            // Assert
            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();
            result.ShouldAllBe(c => c.Name.Contains("Buenos Aires", StringComparison.OrdinalIgnoreCase));

        }

        [Fact]
        public async Task SearchByName_Should_Return_Empty_When_No_Matches()
        {
            
            // Act
            var result = await _service.SearchCitiesByNameAsync("XYZInvalidCity123");

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }


        // ----------------------- Pruebas para el metodo FilterCities --------------------------

        [Fact]
        public async Task Filter_Should_Return_Cities_By_Country()
        {
            
            // Act
            var result = await _service.FilterCitiesAsync("AR", 0, null);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();
            result.First().Country.ShouldContain("Argentina"); ;

        }


        [Fact]
        public async Task Filter_Should_Combine_Multiple_Filters()
        {
           
            // Act
            var result = await _service.FilterCitiesAsync("AR", 500000, "Buenos");

            // Assert
            result.ShouldNotBeNull();
            if (result.Any())
            {
                result.ShouldAllBe(c =>
                    c.Population >= 500000 &&
                    c.Name.Contains("Buenos", StringComparison.OrdinalIgnoreCase));      
            }
        }

        [Fact]
        public async Task Filter_Should_Return_Empty_When_No_Matches()
        {
           
            // Act
            var result = await _service.FilterCitiesAsync("XX", 99999999, "NonExistent");

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task Filter_Should_Handle_Null_Optional_Parameters()
        {
            // Act & Assert - No deberia lanzar excepcion
            var result = await _service.FilterCitiesAsync(null, 0, null);

            result.ShouldNotBeNull();   
            // Deberia retornar alguna resultado por defecto o vacio.
        }


        // ------------------------ Pruebas para el metodo GetDetailsAsync ----------------------------

        [Fact]
        public async Task GetCityDetails_Should_Return_Complete_Information()
        {
            // Arrange - Usamos un id conocido de antemano (Buenos Aires)
            var cityId = 3435910;

            // Act
            var result = await _service.GetCityDetailsAsync(cityId);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(cityId);
            result.Name.ShouldNotBeNullOrWhiteSpace();
            result.Country.ShouldNotBeNullOrWhiteSpace();
            result.CountryCode.ShouldNotBeNullOrWhiteSpace();
            result.Region.ShouldNotBeNullOrWhiteSpace();
            result.Latitude.ShouldBeInRange(-90, 90);
            result.Longitude.ShouldBeInRange(-180, 180);
            result.Population.ShouldBeGreaterThanOrEqualTo(0);
            result.TimeZone.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task GetCityDetails_Should_Throw_When_City_Not_Found()
        {
            // Act & Assert
            await Should.ThrowAsync<EntityNotFoundException>(async () =>
            {
                await _service.GetCityDetailsAsync(99999999);
            });
        }

    }
}
