using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace TurisGo.Destinos
{
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

        [Fact]
        public async Task SearchByName_Should_Map_All_Required_Properties()
        {
            
            // Act
            var result = await _service.SearchCitiesByNameAsync("Roma");

            // Assert
            result.ShouldNotBeNull();
            if (result.Any())
            {
                var city = result.First();
                city.Name.ShouldNotBeNullOrWhiteSpace();
                city.Country.ShouldNotBeNullOrWhiteSpace();
                city.Latitude.ShouldBeInRange(-90, 90);
                city.Longitude.ShouldBeInRange(-180, 180);
                city.Population.ShouldBeGreaterThanOrEqualTo(0);
            }
        }

        [Fact]
        public async Task SearchByName_Should_Handle_Special_Characters()
        {
            
            // Act
            var result = await _service.SearchCitiesByNameAsync("São Paulo");

            // Assert
            result.ShouldNotBeNull();
            // Deberia manejar caracteres especiales sin errores
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
        public async Task Filter_Should_Return_Cities_By_MinPopulation()
        {
            

            // Act
            var result = await _service.FilterCitiesAsync(null, 2000000, null);

            // Assert
            result.ShouldNotBeNull();
            if (result.Any())
            {
                result.ShouldAllBe(c => c.Population >= 2000000);
            }
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

        [Fact]
        public async Task Filter_Should_Validate_Country_Code_Format()
        {
            // Act
            var result = await _service.FilterCitiesAsync("INVALID", 0, null);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty(); 
        }
        

        

    }
}
