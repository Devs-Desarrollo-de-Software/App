using Abp.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisGo.Destinos;
using TurisGo.EntityFrameworkCore;
using TurisGo.Usuarios;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Clients;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using Volo.Abp.Validation;

namespace TurisGo.Calificaciones
{
    [Authorize]
    public class CalificacionAppService : ApplicationService, ICalificacionAppService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Calificacion, Guid> _repository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly IRepository<Destino, Guid> _destinoRepository;

        public CalificacionAppService(
            IRepository<Calificacion, Guid> repository,
            ICurrentUser currentUser,
            IRepository<IdentityUser, Guid> userRepository,
            IRepository<Destino, Guid> destinoRepository)
        {
            _currentUser = currentUser;
            _repository = repository;
            _userRepository = userRepository;
            _destinoRepository = destinoRepository;
        }


        // Crea una nueva calificación para un destino
        public async Task<CalificacionDto> CreateAsync(CreateCalificacionDto input)
        {
            if (!_currentUser.IsAuthenticated)
                throw new UnauthorizedAccessException("Debe estar autenticado para calificar un destino.");

            var userId = _currentUser.Id!.Value;

            // Verificar si el usuario ya calificó este destino anteriormente
            var yaCalifico = await _repository.FirstOrDefaultAsync(x =>
                x.DestinoId == input.DestinoId &&
                x.UserId == userId);

            if (yaCalifico != null)
                throw new AbpValidationException("Ya has calificado este destino.");

            // Crear la nueva calificación
            var calificacion = new Calificacion(
                GuidGenerator.Create(),
                input.DestinoId,
                userId,
                input.Puntuacion,
                input.Comentario
            );

            var calificacionCreada = await _repository.InsertAsync(calificacion, autoSave: true);
            return ObjectMapper.Map<Calificacion, CalificacionDto>(calificacionCreada);
        }

        // Obtiene una calificación específica por ID
        public async Task<CalificacionDto> GetAsync(Guid id)
        {
            var calificacion = await _repository.GetAsync(id);
            return ObjectMapper.Map<Calificacion, CalificacionDto>(calificacion);
        }

        // Lista las calificaciones del usuario autenticado con paginación
        public async Task<PagedResultDto<CalificacionDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var queryable = await _repository.GetQueryableAsync();

            var query = queryable
                .OrderByDescending(x => x.CreationTime)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            var totalCount = await AsyncExecuter.CountAsync(queryable);
            var calificaciones = await AsyncExecuter.ToListAsync(query);

            var calificacionesDto = ObjectMapper.Map<List<Calificacion>, List<CalificacionDto>>(calificaciones);

            // Agregar el nombre del destino a cada calificación
            foreach (var calificacionDto in calificacionesDto)
            {
                var destino = await _destinoRepository.FirstOrDefaultAsync(d => d.Id == calificacionDto.DestinoId);
                calificacionDto.DestinoNombre = destino?.Nombre ?? "Destino eliminado";
            }

            return new PagedResultDto<CalificacionDto>(
                totalCount,
                calificacionesDto
            );
        }

        // 5.3. Editar calificacion propia.
        public async Task <CalificacionDto> UpdateAsync(Guid id, UpdateCalificacionDto input)
        {

            if (!_currentUser.IsAuthenticated)
                throw new UnauthorizedAccessException("Debe estar autenticado para actualizar!");

            var userId = _currentUser.Id!.Value;

            // Buscar la calificacion
            var calificacion = await _repository.GetAsync(id);

            if (calificacion.UserId != userId)
                throw new AbpAuthorizationException("No tiene permisos para editar esta calificación.");

            // Actualizar los datos
            calificacion.SetPuntuacion(input.Puntuacion);
            calificacion.SetComentario(input.Comentario);

            // Guardar cambios
            var calificacionAutualizada = await _repository.UpdateAsync(calificacion, autoSave: true);
            return ObjectMapper.Map<Calificacion, CalificacionDto>(calificacionAutualizada);

        }

        // 5.4 Consultar promedio de calificaciones de un destino (GLOBAL)
        [Authorize]
        [HttpGet]
        [Route("promedio/{destinoId}")]
        public async Task<PromedioCalificacionDto> GetPromedioAsync(Guid destinoId)
        {
            if (destinoId == Guid.Empty)
                throw new AbpValidationException("El ID del destino no puede ser nulo.");

            var queryable = await _repository.GetQueryableAsync();

            // Ignoramos el filtro global SOLO para el cálculo agregado
            var calificacionesQuery = queryable
                .IgnoreQueryFilters()
                .Where(c => c.DestinoId == destinoId);

            var totalCalificaciones = await AsyncExecuter.CountAsync(calificacionesQuery);

            if (totalCalificaciones == 0)
            {
                return new PromedioCalificacionDto
                {
                    DestinoId = destinoId,
                    PromedioCalificacion = 0,
                    TotalCalificaciones = 0
                };
            }

            var promedio = await AsyncExecuter.AverageAsync(
                calificacionesQuery,
                c => c.Puntuacion
            );

            return new PromedioCalificacionDto
            {
                DestinoId = destinoId,
                PromedioCalificacion = Math.Round(promedio, 2),
                TotalCalificaciones = totalCalificaciones
            };
        }

        // 5.5 Listar comentarios de un destino (privados - solo del usuario autenticado)
        public async Task<ListarComentariosDto> GetListComentariosAsync(Guid destinoId)
        {
            if (destinoId == Guid.Empty)
                throw new AbpValidationException("El ID del destino no puede ser nulo.");

            if (!_currentUser.IsAuthenticated)
                throw new AbpAuthorizationException("Debe estar autenticado.");

            var userId = _currentUser.Id!.Value;
            var queryable = await _repository.GetQueryableAsync();

            // Filtrar solo comentarios del usuario autenticado (privacidad)
            var comentarios = await AsyncExecuter.ToListAsync(
                queryable
                    .Where(c => c.DestinoId == destinoId && c.UserId == userId)
                    .OrderByDescending(c => c.CreationTime)
            );

            return new ListarComentariosDto
            {
                DestinoId = destinoId,
                Comentarios = comentarios.Select(c => new ComentarioDto
                {
                    Puntuacion = c.Puntuacion,
                    Comentario = c.Comentario,
                    CreationTime = c.CreationTime
                }).ToList()
            };
        }

        // 5.3 Eliminar calificacion propia
        public async Task DeleteAsync(Guid id)
        {
            if (!_currentUser.IsAuthenticated)
                throw new UnauthorizedAccessException("Debe estar autenticado para eliminar una calificación");

            var userId = _currentUser.Id!.Value;

            // Buscar la calificacion
            var calificacion = await _repository.GetAsync(id);

            // Verificar que la calificacion pertenece al usuario actual
            if (calificacion.UserId != userId)
                throw new AbpAuthorizationException("No tiene permisos para eliminar esta calificación");

            // Eliminar la calificacion
            await _repository.DeleteAsync(id, autoSave: true);
        }

        
    }
}
