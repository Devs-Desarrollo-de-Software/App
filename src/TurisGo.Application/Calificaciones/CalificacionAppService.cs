using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisGo.Destinos;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Clients;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Volo.Abp.Validation;

namespace TurisGo.Calificaciones
{
   [Authorize] //Exige token
    public class CalificacionAppService :
        CrudAppService
        <
          Calificacion,
          CalificacionDto,
          Guid,
          PagedAndSortedResultRequestDto,
          CreateUpdateCalificacionDto
        >,
        ICalificacionAppService
    {
        private readonly ICurrentUser _currentUser;

        public CalificacionAppService(
            IRepository<Calificacion, Guid> repository,                                  
            ICurrentUser currentUser) 
            : base(repository)
        {
            _currentUser = currentUser;                          
        }


        public override async Task<CalificacionDto> CreateAsync(CreateUpdateCalificacionDto input)
        {
            if (!_currentUser.IsAuthenticated)
                throw new UnauthorizedAccessException("Debe estar autenticado para calificar un destino.");

            var userId = _currentUser.Id!.Value;

            // Verificar si el usuario ya califico anteriormente
            var yaCalifico = await Repository.FirstOrDefaultAsync(x =>
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

            var calificacionCreada = await Repository.InsertAsync(calificacion, autoSave: true);
            return ObjectMapper.Map<Calificacion, CalificacionDto>(calificacionCreada);
        }
    }
}
