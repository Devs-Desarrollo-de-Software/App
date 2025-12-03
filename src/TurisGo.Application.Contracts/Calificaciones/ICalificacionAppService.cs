using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace TurisGo.Calificaciones
{
    public interface ICalificacionAppService : IApplicationService
    {
        Task<CalificacionDto> CreateAsync(CreateCalificacionDto input);
        Task<CalificacionDto> UpdateAsync(Guid id, UpdateCalificacionDto input);
        Task<CalificacionDto> GetAsync(Guid id);
        Task<PagedResultDto<CalificacionDto>> GetListAsync(PagedAndSortedResultRequestDto input);

        Task DeleteAsync(Guid id);
    }
}
