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
    }
}
