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
        Task<CalificacionDto> CreateAsync(CreateCalificacionDto input); // 5.1. Calificar un destino (1 a 5 estrellas) && 5.2. Agregar un comentario junto con la calificacion.
        Task<CalificacionDto> UpdateAsync(Guid id, UpdateCalificacionDto input);    // 5.3. Editar una calificacion propia.
        Task<CalificacionDto> GetAsync(Guid id);
        Task<PagedResultDto<CalificacionDto>> GetListAsync(PagedAndSortedResultRequestDto input);
        Task DeleteAsync(Guid id);  // 5.3. Eliminar un destino propio.
        Task<PromedioCalificacionDto> GetPromedioAsync(Guid destinoId); // 5.4. Obtener promedio de calificacion de un destino
        Task<ListarComentariosDto> GetListComentariosAsync(Guid destinoId); // 5.5. Listar comentarios de un destino.
    }
}
