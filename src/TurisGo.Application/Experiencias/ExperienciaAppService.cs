using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using TurisGo.Calificaciones;
using TurisGo.Destinos;
using Volo.Abp;  
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;  
using Volo.Abp.Validation;

namespace TurisGo.Experiencias
{
    [Authorize]
    public class ExperienciaAppService : ApplicationService, IExperienciaAppService
    {
        private readonly IRepository<Experiencia, Guid> _repository;
        private readonly IRepository<Destino, Guid> _destinoRepository;

        public ExperienciaAppService(
            IRepository<Experiencia, Guid> repository,
            IRepository<Destino, Guid> destinoRepository)
        {
            _repository = repository;
            _destinoRepository = destinoRepository; 
        }

        public async Task<ExperienciaDto> CreateAsync (CreateExperienciaDto input)
        {
            var userId = CurrentUser.Id!.Value;

            // Validar que el destino exista
            var destinoExiste = await _destinoRepository.FindAsync(input.DestinoId);

            if (destinoExiste == null)
            {
                throw new BusinessException("El destino especificado no existe.");
            }

            var expDuplicada = await _repository.FirstOrDefaultAsync(x =>
                x.DestinoId == input.DestinoId &&
                x.UserId == userId);

            if (expDuplicada != null)
            {
                throw new AbpValidationException("Ya has creado una experiencia para este destino.");
            }

            var experiencia = new Experiencia(
                GuidGenerator.Create(),
                userId,
                input.DestinoId,
                input.Titulo,
                input.Descripcion,
                input.Valoracion
            );

            var experienciaCreada = await _repository.InsertAsync(experiencia, autoSave: true);

            return ObjectMapper.Map<Experiencia, ExperienciaDto>(experienciaCreada);

        }


        public async Task<ExperienciaDto> UpdateAsync(Guid id, UpdateExperienciaDto input)
        {

            var userId = CurrentUser.Id!.Value;

            // Verificar que ya califico anteriormente.
            var experiencia = await _repository.GetAsync(id);
            if (experiencia == null)
            {
                throw new BusinessException("El ID no corresponde a ninguna experiencia.");
            }

            if (experiencia.UserId != userId)
                throw new AbpAuthorizationException("No tiene permisos para editar esta calificación.");

            // Actualizamos experiencia
            experiencia.SetTitulo(input.Titulo);
            experiencia.SetDescripcion(input.Descripcion);
            experiencia.SetValoracion(input.Valoracion);

            // Agregar la experiencia a la BD
            var experienciaCreada = await _repository.UpdateAsync(experiencia, autoSave: true);


            // Mapear y devolver un DTO
            return ObjectMapper.Map<Experiencia, ExperienciaDto>(experienciaCreada);

        }

        public async Task DeleteAsync(Guid id)
        {
            var userId = CurrentUser.Id!.Value;
            var experiencia = await _repository.FirstOrDefaultAsync(c => 
                                        c.Id == id &&
                                        c.UserId == userId);

            if (experiencia == null)
            {
                throw new BusinessException("El ID no corresponde a ninguna experiencia propia.");
            }

            await _repository.DeleteAsync(experiencia, autoSave: true);

        }

        





    }
}
