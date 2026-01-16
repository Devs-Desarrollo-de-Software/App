using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisGo.Destinos
{
    public interface ICitySearchService
    {
        Task<List<CityDto>> SearchCitiesByNameAsync(string namePrefix);
        Task<List<CityDto>> FilterCitiesAsync(string paisPrefix, int poblacionMin, string regionPrefix, string cityNamePrefix = null);
        Task<CityDetailDto> GetCityDetailsAsync(int cityId);
        Task<List<CityDto>> GetPopularCitiesAsync(int limit = 10);
    }
}
