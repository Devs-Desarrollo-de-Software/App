using Abp.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
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
using Volo.Abp.Users;
using Volo.Abp.Validation;

namespace TurisGo.Calificaciones
{
   [Authorize] //Exige token
    public class CalificacionAppService : ApplicationService, ICalificacionAppService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Calificacion, Guid> _repository;


        public CalificacionAppService(
            IRepository<Calificacion, Guid> repository,                                  
            ICurrentUser currentUser)
        {
            _currentUser = currentUser;   
            _repository = repository;
        }


        public async Task<CalificacionDto> CreateAsync(CreateCalificacionDto input)
        {
            if (!_currentUser.IsAuthenticated)
                throw new UnauthorizedAccessException("Debe estar autenticado para calificar un destino.");

            var userId = _currentUser.Id!.Value;

            // Verificar si el usuario ya califico anteriormente
            var yaCalifico = await _repository.FirstOrDefaultAsync(x =>
                x.DestinoId == input.DestinoId &&
                x.UserId == userId);

            if (yaCalifico != null)
                throw new AbpValidationException("Ya has calificado este destino.");

            //Crear la nueva calificacion
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

        public async Task<CalificacionDto> GetAsync(Guid id)
        {
            var calificacion = await _repository.GetAsync(id);
            return ObjectMapper.Map<Calificacion, CalificacionDto>(calificacion);
        }

        public async Task<PagedResultDto<CalificacionDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var queryable = await _repository.GetQueryableAsync();

            var query = queryable
                .OrderByDescending(x => x.CreationTime)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            var totalCount = await AsyncExecuter.CountAsync(queryable);
            var calificaciones = await AsyncExecuter.ToListAsync(query);

            return new PagedResultDto<CalificacionDto>(
                totalCount,
                ObjectMapper.Map<List<Calificacion>, List<CalificacionDto>>(calificaciones)
            );
        }

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

        [AllowAnonymous]    // Permitir que cualquier usuario consulte.
        public async Task<PromedioCalificacionDto> GetPromedioAsync(Guid destinoId)
        {
            if (destinoId == Guid.Empty)
            {
                throw new AbpValidationException("El ID del destino no puede ser nulo.");
            }

            var queryable = await _repository.GetQueryableAsync();

            var calificaciones = await AsyncExecuter.ToListAsync(
                queryable
                    .IgnoreQueryFilters()
                    .Where(c => c.DestinoId == destinoId)
            );

                if (!calificaciones.Any())
                {
                    return new PromedioCalificacionDto
                    {
                        DestinoId = destinoId,
                        PromedioCalificacion = 0,
                        TotalCalificaciones = 0
                    };
                }

                // Calcular el promedio
                var promedio = calificaciones.Average(c => c.Puntuacion);
                var total = calificaciones.Count;

                return new PromedioCalificacionDto
                {
                    DestinoId = destinoId,
                    PromedioCalificacion = Math.Round(promedio, 2),
                    TotalCalificaciones = total
                };
            

        }

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
