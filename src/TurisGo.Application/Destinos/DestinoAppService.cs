using Microsoft.AspNetCore.Authorization;
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
using Volo.Abp.Validation;

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
                throw new AbpValidationException("El nombre de la ciudad no puede estar vacio.");
            
            return await _citySearchService.SearchCitiesByNameAsync(nombre);
        }

        [HttpGet]
        [Route("api/app/destino/buscar-ciudades-por-filtro")]
        public async Task<List<CityDto>> FiltrarCiudadesAsync(string paisPrefix = null, int poblacionMin = 0, string regionPrefix = null)
        {
            // Si no viene ningun filtro, devolemos lista vacia.
            if (paisPrefix.IsNullOrEmpty() && regionPrefix.IsNullOrEmpty() && poblacionMin <= 0)
            {
                return new List<CityDto>(); // Retorna lista vacia.
            }

            if (poblacionMin < 0)
                throw new AbpValidationException("La poblacion no debe ser negativa.");

            return await _citySearchService.FilterCitiesAsync(paisPrefix, poblacionMin, regionPrefix);
        }

        [HttpGet]
        [Route("api/app/destino/obtener-info-ciudad/{cityId}")]
        public async Task<CityDetailDto> ObtenerDetalleCiudadAsync(int cityId)
        {
            if (cityId <= 0)
                throw new AbpValidationException("El Id debe ser mayor a cero.");

            return await _citySearchService.GetCityDetailsAsync(cityId);
        }

        [Authorize]
        [HttpPost]
        [Route("api/app/destino/guardar-desde-api/{cityId}")]
        public async Task<DestinoDto> GuardarDestinoDesdeApiAsync(int cityId)
        {
            if (cityId <= 0)
                throw new AbpValidationException("El Id de la ciudad debe ser mayor a cero.");

            // 1️⃣ Obtener info desde API externa
            var city = await _citySearchService.GetCityDetailsAsync(cityId);

            // 2️⃣ Verificar si ya existe en la base
            var existente = await Repository.FirstOrDefaultAsync(
                x => x.Nombre == city.Name && x.Pais == city.Country);

            if (existente != null)
                throw new BusinessException("DestinoYaExiste")
                    .WithData("Nombre", city.Name)
                    .WithData("Pais", city.Country);

            // 3️⃣ Crear entidad de dominio
            var destino = new Destino(
                GuidGenerator.Create(),
                city.Name,
                city.Country,
                city.Population,
                ObtenerImagenPorDefecto(city), // ver método abajo
                new Coordenada(city.Latitude, city.Longitude)
            );

            // 4️⃣ Guardar en DB interna
            await Repository.InsertAsync(destino, autoSave: true);

            // 5️⃣ Devolver DTO
            return ObjectMapper.Map<Destino, DestinoDto>(destino);
        }

        private string ObtenerImagenPorDefecto(CityDetailDto city)
        {
            return $"https://placehold.co/600x400?text={Uri.EscapeDataString(city.Name)}";
        }


    }

}
        
