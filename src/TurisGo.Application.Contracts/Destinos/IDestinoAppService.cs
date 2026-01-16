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
        Task<List<CityDto>> BuscarCiudadesPorNombreAsync(string nombre);

        Task<List<CityDto>> FiltrarCiudadesAsync(
            string paisPrefix = null,
            int poblacionMin = 0,
            string regionPrefix = null,
            string nombreCiudad = null);

        Task<CityDetailDto> ObtenerDetalleCiudadAsync(int cityId);

        Task<DestinoDto> GuardarDestinoDesdeApiAsync(int cityId);
        Task<List<CityDto>> GetDestinosPopularesAsync();
    }
}
