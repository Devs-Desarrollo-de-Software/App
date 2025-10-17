using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
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


        [Fact]
        public async Task Should_Return_CityList()
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
        public async Task Should_Throw_Exception_When_NameIsEmpty()
        {
            //Act & Assert
            await Should.ThrowAsync<ArgumentException>(async () =>
            {
                await _destinoAppService.BuscarCiudadesPorNombreAsync("");
            });
        }


        [Fact]
        public async Task Should_Return_EmptyList_When_NoResults()
        {
            //Arrange
            _mockCityService
                .Setup(x => x.SearchCitiesByNameAsync("CiudadInventada"))
                .ReturnsAsync(new List<CityDto>());

            //Act
            var result = await _destinoAppService.BuscarCiudadesPorNombreAsync("CiudadInventada");

            //Assert
            result.ShouldNotBeNull();

        }

        [Fact]
        public async Task Should_Throw_Exception_When_ApiFails()
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

    }
    
}
