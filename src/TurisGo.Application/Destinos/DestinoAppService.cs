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
        private readonly ICitySearchService _citySearchService;
        public DestinoAppService(
            IRepository<Destino, Guid> repository,
            ICitySearchService citySearchService)           
            : base(repository)
        {
            _citySearchService = citySearchService;
        }

        [HttpGet]
        [Route("api/app/destino/buscar-ciudad-por-nombre")]
        public async Task<List<CityDto>> BuscarCiudadesPorNombreAsync(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la ciudad no puede estar vacio.");
            return await _citySearchService.SearchCitiesByNameAsync(nombre);
        }

    }

}
        
