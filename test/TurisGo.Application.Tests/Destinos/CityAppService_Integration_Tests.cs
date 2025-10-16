using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        [Fact]
        public async Task Should_Map_Response_To_CityDto_Correctly()
        {

            //Act
            var result = await _service.SearchCitiesByNameAsync("Roma");

            //Assert
            // Verifica que los datos se mapeen correctamente   
            result.ShouldNotBeNull();
            result[0].Name.ShouldContain("Roma");
            result[0].Country.ShouldNotBeNullOrWhiteSpace();
            result[0].Population.ShouldBeGreaterThan(-1);
            result[0].Latitude.ShouldBeInRange(-90, 90);   
            result[0].Longitude.ShouldBeInRange(-180, 180);

            result[0].ShouldBeAssignableTo<CityDto>();
        }

        [Fact]
        public async Task Should_Handle_Http_Error_Gracefully()
        {
            //Arrange
            var result = await _service.SearchCitiesByNameAsync("InvalidTest");

            //Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

    }
}
