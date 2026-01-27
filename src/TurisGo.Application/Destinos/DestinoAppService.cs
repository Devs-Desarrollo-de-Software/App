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

        // Repositorio de calificaciones para calcular destinos populares
        private readonly IRepository<TurisGo.Calificaciones.Calificacion, Guid> _calificacionRepository;

        public DestinoAppService(
            IRepository<Destino, Guid> repository,
            ICitySearchService citySearchService,
            IRepository<TurisGo.Calificaciones.Calificacion, Guid> calificacionRepository)
            : base(repository)
        {
            _citySearchService = citySearchService;
            _calificacionRepository = calificacionRepository;
        }

        // Busca ciudades en la API externa por nombre
        [HttpGet]
        [Route("api/app/destino/buscar-ciudad-por-nombre")]
        public async Task<List<CityDto>> BuscarCiudadesPorNombreAsync(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new AbpValidationException("El nombre de la ciudad no puede estar vacio.");

            // Normalizar: primera letra mayúscula, resto minúscula
            var nombreNormalizado = NormalizarTexto(nombre);

            return await _citySearchService.SearchCitiesByNameAsync(nombreNormalizado);
        }

        // Filtra ciudades en la API externa por múltiples criterios
        // Permite filtrar por país, población mínima, región y nombre
        [HttpGet]
        [Route("api/app/destino/buscar-ciudades-por-filtro")]
        public async Task<List<CityDto>> FiltrarCiudadesAsync(string paisPrefix = null, int poblacionMin = 0, string regionPrefix = null, string nombreCiudad = null)
        {
            // Validar que la población no sea negativa
            if (poblacionMin < 0)
                throw new AbpValidationException("La poblacion no debe ser negativa.");

            // Normalizar los filtros de texto (case-insensitive)
            var paisNormalizado = NormalizarTexto(paisPrefix);
            var regionNormalizada = NormalizarTexto(regionPrefix);
            var ciudadNormalizada = NormalizarTexto(nombreCiudad);

            return await _citySearchService.FilterCitiesAsync(paisNormalizado, poblacionMin, regionNormalizada, ciudadNormalizada);
        }

        // Obtiene información detallada de una ciudad específica desde la API externa
        [HttpGet]
        [Route("api/app/destino/obtener-info-ciudad/{cityId}")]
        public async Task<CityDetailDto> ObtenerDetalleCiudadAsync(int cityId)
        {
            if (cityId <= 0)
                throw new AbpValidationException("El Id debe ser mayor a cero.");

            return await _citySearchService.GetCityDetailsAsync(cityId);
        }

        // Guarda un destino desde la API externa a la base de datos local
        [Authorize]
        [HttpPost]
        [Route("api/app/destino/guardar-desde-api/{cityId}")]
        public async Task<DestinoDto> GuardarDestinoDesdeApiAsync(int cityId)
        {
            if (cityId <= 0)
                throw new AbpValidationException("El Id de la ciudad debe ser mayor a cero.");

            // Obtener información completa de la ciudad desde la API externa
            var city = await _citySearchService.GetCityDetailsAsync(cityId);

            // Verificar si el usuario ya guardó este destino previamente
            var userId = CurrentUser.Id;
            var existente = await Repository.FirstOrDefaultAsync(
                x => x.Nombre == city.Name && x.Pais == city.Country && x.CreatorId == userId);

            // Si ya existe para este usuario, devolver el existente sin duplicar
            if (existente != null)
            {
                return ObjectMapper.Map<Destino, DestinoDto>(existente);
            }

            // Crear la entidad de dominio con los datos de la API
            var destino = new Destino(
                GuidGenerator.Create(),
                city.Name,
                city.Country,
                city.Population,
                ObtenerImagenPorDefecto(city),
                new Coordenada(city.Latitude, city.Longitude),
                cityId  // Guardar el ID de la API externa para referencia
            );

            // Guardar el destino en la base de datos local
            await Repository.InsertAsync(destino, autoSave: true);

            // Devolver el DTO del destino creado
            return ObjectMapper.Map<Destino, DestinoDto>(destino);
        }

        // Genera una URL de imagen placeholder personalizada con el nombre de la ciudad
        private string ObtenerImagenPorDefecto(CityDetailDto city)
        {
            return $"https://placehold.co/600x400?text={Uri.EscapeDataString(city.Name)}";
        }

        // Normaliza texto para búsquedas case-insensitive
        // Convierte a formato: Primera letra mayúscula, resto minúscula
        // Ej: "cOlON" -> "Colon", "ARGENTINA" -> "Argentina"
        private string NormalizarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return null;

            texto = texto.Trim();

            // Convertir primera letra a mayúscula, resto a minúscula
            return char.ToUpper(texto[0]) + texto.Substring(1).ToLower();
        }

        // Obtiene los destinos más populares basándose en su calificación promedio
        // Los destinos se ordenan por calificación y en caso de empate, por población
        [HttpGet]
        [Route("api/app/destino/populares")]
        public async Task<List<CityDto>> GetDestinosPopularesAsync(int limit = 10)
        {
            // Obtener todos los destinos guardados en la base de datos
            var destinos = await Repository.GetListAsync();

            if (!destinos.Any())
            {
                // Si no hay destinos guardados, devolver lista vacía
                return new List<CityDto>();
            }

            // Obtener todas las calificaciones y calcular el promedio por destino
            var calificaciones = await _calificacionRepository.GetListAsync();

            var promediosPorDestino = calificaciones
                .GroupBy(c => c.DestinoId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Average(c => c.Puntuacion)
                );

            // Combinar destinos con sus promedios y ordenar
            var destinosConPromedio = destinos.Select(d => new
            {
                Destino = d,
                Promedio = promediosPorDestino.ContainsKey(d.Id) ? promediosPorDestino[d.Id] : 0
            })
            .OrderByDescending(x => x.Promedio)
            .ThenByDescending(x => x.Destino.Poblacion) // Desempate por población
            .Take(limit)
            .ToList();

            // Convertir a CityDto para mantener compatibilidad con el frontend
            return destinosConPromedio.Select(x => new CityDto
            {
                Id = x.Destino.ApiCityId,
                Name = x.Destino.Nombre,
                Country = x.Destino.Pais,
                Population = x.Destino.Poblacion,
                Latitude = x.Destino.Coordenada.Latitud,
                Longitude = x.Destino.Coordenada.Longitud
            }).ToList();
        }
    }
}
