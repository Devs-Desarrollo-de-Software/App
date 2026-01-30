using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace TurisGo.Notificaciones
{
    public interface INotificacionAppService : IApplicationService
    {
        // [ADMIN] Notifica a todos los usuarios que tienen el destino en favoritos sobre un cambio relevante
        Task<NotificacionResultDto> NotificarCambioDestinoAsync(
             Guid destinoId,
             string titulo,
             string mensaje,
             TipoNotificacion tipo);

        // [USUARIO] Obtiene la lista de notificaciones del usuario actual
        Task<PagedResultDto<NotificacionDto>> GetListAsync(GetNotificacionesInput input);


        // [USUARIO] Marca una notificación como leída o no leída
        Task MarcarComoLeidaAsync(Guid id, bool leida);


        // [USUARIO] Obtiene el conteo de notificaciones no leídas
        Task<int> GetConteoNoLeidasAsync();


    }
}
