using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using TurisGo.Destinos;
using Volo.Abp;  
using Volo.Abp.Application.Services;  
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


    }
}
