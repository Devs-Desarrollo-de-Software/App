using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace TurisGo.Experiencias
{
    public interface IExperienciaAppService : IApplicationService
    {
        Task<ExperienciaDto> CreateAsync(CreateExperienciaDto input); // 4.1. Crear una nueva experiencia de un destino.
        Task<ExperienciaDto> UpdateAsync(Guid Id, UpdateExperienciaDto input); // 4.2. Editar una experiencia propia.
        Task DeleteAsync(Guid id); // 4.3. Eliminar una experiencia propia.
        Task<ListarExperienciasDto> GetListExperienciasAsync(Guid destinoId); // 4.4. Listar experiencias de un destino.

        // 4.5 y 4.6. Filtrar experiencias por valoración y/o buscar por palabras clave.
        Task<PagedResultDto<ExperienciaDto>> GetListAsync(GetExperienciasListDto input); 
    }
}
