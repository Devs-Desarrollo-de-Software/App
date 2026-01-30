using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisGo.Destinos
{
    public interface ICitySearchService
    {
        Task<List<CityDto>> SearchCitiesByNameAsync(string namePrefix);  // 3.1. Buscar ciudades por nombre.

        // 3.2. Buscar ciudades filtrando por país, región o población mínima.
        Task<List<CityDto>> FilterCitiesAsync(string paisPrefix, int poblacionMin, string regionPrefix, string cityNamePrefix = null);  
        Task<CityDetailDto> GetCityDetailsAsync(int cityId);        // 3.3. Obtener información detallada de una ciudad.
        Task<List<CityDto>> GetPopularCitiesAsync(int limit = 10);
    }
}
