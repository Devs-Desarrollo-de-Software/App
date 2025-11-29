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

        [HttpGet]
        [Route("api/app/destino/buscar-ciudades-por-filtro")]
        public async Task<List<CityDto>> FiltrarCiudadesAsync(string paisPrefix = null, int poblacionMin = 0, string regionPrefix = null)
        {
            if (paisPrefix.IsNullOrEmpty() && regionPrefix.IsNullOrEmpty() && poblacionMin <= 0)
            {
                return new List<CityDto>(); // Retorna lista vacia.
            }

            if (poblacionMin < 0)
                throw new ArgumentException("La poblacion no debe ser negativa", nameof(poblacionMin));

            return await _citySearchService.FilterCitiesAsync(paisPrefix, poblacionMin, regionPrefix);
        }

        

    }

}
        
