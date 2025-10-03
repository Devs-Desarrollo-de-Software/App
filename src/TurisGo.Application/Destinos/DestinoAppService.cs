using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace TurisGo.Destinos
{
    public class DestinoAppService :
        CrudAppService
        <
         Destino,
         DestinoDto,
         Guid,
         PagedAndSortedResultRequestDto,
         CreateUpdateDestinoDto>,
         IDestinoAppService
        

    {
        public DestinoAppService(IRepository<Destino, Guid> repository) : base(repository) { }


        public override async Task<DestinoDto> CreateAsync(CreateUpdateDestinoDto input)
        {
         
            //Validacion de negocio: No duplicar destinos
            var existe = await Repository.AnyAsync(
                d => d.Nombre == input.Nombre && d.Pais == input.Pais
                );

            if (existe)
            {
                throw new BusinessException("Destino.Duplicado")
                    .WithData("Nombre", input.Nombre)
                    .WithData("Pais", input.Pais);
            }

            //No duplicar coordenadas
            var igualCoord = await Repository.AnyAsync(
                d => d.Coordenada.Latitud == input.Coordenada.Latitud &&
                     d.Coordenada.Longitud == input.Coordenada.Longitud
            );

            if (igualCoord)
            {
                throw new BusinessException("Destino.CoordenadaDuplicada");
            }

            return await base.CreateAsync(input);

        
        }

    }
        
}
