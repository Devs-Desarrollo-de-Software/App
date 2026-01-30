using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace TurisGo.Destinos
{
    public interface IDestinoAppService:
    ICrudAppService
        <
        DestinoDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateDestinoDto
        >
    {
        Task<List<CityDto>> BuscarCiudadesPorNombreAsync(string nombre);    // 3.1. Buscar ciudades por nombre

        Task<List<CityDto>> FiltrarCiudadesAsync(   
            string paisPrefix = null,
            int poblacionMin = 0,
            string regionPrefix = null,
            string nombreCiudad = null);    // 3.2. Buscar ciudades filtrando por país, región o población mínima.

        Task<CityDetailDto> ObtenerDetalleCiudadAsync(int cityId);  // 3.3. Obtener información detallada de una ciudad.

        Task<DestinoDto> GuardarDestinoDesdeApiAsync(int cityId);      //  3.5. Guardar destinos en la base interna de la app
        Task<List<CityDto>> GetDestinosPopularesAsync(int limit = 10);
    }
}
