using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurisGo.Destinos;
using TurisGo.Favoritos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Data; // Para DataFilter
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace TurisGo.Notificaciones
{
    [Authorize]
    public class NotificacionAppService : ApplicationService, INotificacionAppService
    {
        private readonly IRepository<Notificacion, Guid> _notificacionRepository;
        private readonly IRepository<Favorito, Guid> _favoritoRepository;
        private readonly IRepository<Destino, Guid> _destinoRepository;
        private readonly IDataFilter _dataFilter;

        public NotificacionAppService(
            IRepository<Notificacion, Guid> notificacionRepository,
            IRepository<Favorito, Guid> favoritoRepository,
            IRepository<Destino, Guid> destinoRepository,
            IDataFilter dataFilter) // ⭐ Inyectar IDataFilter
        {
            _notificacionRepository = notificacionRepository;
            _favoritoRepository = favoritoRepository;
            _destinoRepository = destinoRepository;
            _dataFilter = dataFilter;
        }

        [Authorize(Roles = "admin")]
        [UnitOfWork]
        public virtual async Task<NotificacionResultDto> NotificarCambioDestinoAsync(
        Guid destinoId,
        string titulo,
        string mensaje,
        TipoNotificacion tipo)
        {
            var destino = await _destinoRepository.GetAsync(destinoId);

            // Necesitamos ver TODOS los favoritos (no solo los del admin)
            var dbSet = await _favoritoRepository.GetDbSetAsync();
            var favoritosDelDestino = await dbSet
                .IgnoreQueryFilters() // Bypass temporal del filtro
                .Where(f => f.DestinoId == destinoId)
                .ToListAsync();

            if (favoritosDelDestino.Count == 0)
            {
                return new NotificacionResultDto
                {
                    UsuariosNotificados = 0,
                    NombreDestino = destino.Nombre,
                    Tipo = tipo,
                    Mensaje = $"No hay usuarios con {destino.Nombre} en favoritos"
                };
            }

            var notificaciones = new List<Notificacion>();

            foreach (var favorito in favoritosDelDestino)
            {
                var notificacion = new Notificacion(
                    GuidGenerator.Create(),
                    favorito.UserId, // UserId del usuario que tiene el favorito
                    destinoId,
                    titulo,
                    mensaje,
                    tipo,
                    destino.Nombre
                );
                notificaciones.Add(notificacion);
            }

            await _notificacionRepository.InsertManyAsync(notificaciones);
            await CurrentUnitOfWork.SaveChangesAsync();

            return new NotificacionResultDto
            {
                UsuariosNotificados = notificaciones.Count,
                NombreDestino = destino.Nombre,
                Tipo = tipo,
                Mensaje = $"Se notificó a {notificaciones.Count} usuario(s) sobre {destino.Nombre}"
            };
        }

        public virtual async Task<PagedResultDto<NotificacionDto>> GetListAsync(GetNotificacionesInput input)
        {
            var userId = CurrentUser.GetId();
            var queryable = await _notificacionRepository.GetQueryableAsync();

            var query = queryable.Where(n => n.UserId == userId);

            if (input.SoloNoLeidas == true)
            {
                query = query.Where(n => !n.Leida);
            }

            if (input.TipoFiltro.HasValue)
            {
                query = query.Where(n => n.Tipo == input.TipoFiltro.Value);
            }

            query = query.OrderByDescending(n => n.CreationTime);
            var totalCount = await AsyncExecuter.CountAsync(query);
            var items = await AsyncExecuter.ToListAsync(
                query.Skip(input.SkipCount).Take(input.MaxResultCount)
            );

            var dtos = items.Select(n => new NotificacionDto
            {
                Id = n.Id,
                DestinoId = n.DestinoId,
                Titulo = n.Titulo,
                Mensaje = n.Mensaje,
                Tipo = n.Tipo,
                Leida = n.Leida,
                FechaCreacion = n.CreationTime,
                NombreDestino = n.NombreDestino
            }).ToList();

            return new PagedResultDto<NotificacionDto>(totalCount, dtos);
        }

        [UnitOfWork]
        public virtual async Task MarcarComoLeidaAsync(Guid id, bool leida)
        {
            var notificacion = await _notificacionRepository.GetAsync(id);

            if (notificacion.UserId != CurrentUser.GetId())
            {
                throw new UnauthorizedAccessException("No puedes modificar esta notificación");
            }

            if (leida)
                notificacion.MarcarComoLeida();
            else
                notificacion.MarcarComoNoLeida();

            await _notificacionRepository.UpdateAsync(notificacion, autoSave: true);
        }

        public virtual async Task<int> GetConteoNoLeidasAsync()
        {
            var userId = CurrentUser.GetId();
            var queryable = await _notificacionRepository.GetQueryableAsync();

            return await AsyncExecuter.CountAsync(
                queryable.Where(n => n.UserId == userId && !n.Leida)
            );
        }
    }
}