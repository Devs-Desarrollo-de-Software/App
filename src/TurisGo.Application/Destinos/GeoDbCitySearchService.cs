using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Scriban.Runtime.Accessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Validation;

namespace TurisGo.Destinos
{
    public class GeoDbCitySearchService: ICitySearchService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeoDbCitySearchService> _logger;
        
        private const string ApiKey = "9332dd950amsh49b8d385752ec23p13d526jsn5974666e3749";// Replace with your actual API key
        private const string BaseUrl = "https://wft-geo-db.p.rapidapi.com/v1/geo";
        

        public GeoDbCitySearchService(HttpClient httpClient, ILogger<GeoDbCitySearchService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // --------------- 3.1 Buscar ciudades por nombre --------------------------
        public async Task<List<CityDto>> SearchCitiesByNameAsync(string namePrefix)
        {
            if (string.IsNullOrWhiteSpace(namePrefix))
                throw new ArgumentException("El nombre de la ciudad no puede estar vacio.");

            try
            {
                var url = 
                    $"{BaseUrl}/cities?namePrefix={Uri.EscapeDataString(namePrefix)}&limit=5";
                    
                _logger.LogWarning("URL enviada a GeoDb: {Url}", url);

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("X-RapidAPI-Key", ApiKey);
                request.Headers.Add("X-RapidAPI-Host", "wft-geo-db.p.rapidapi.com");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error al consultar GeoDb: {response.StatusCode}");

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GeoDbResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data.Select(c => new CityDto
                {
                    Id = c.Id,
                    Name = c.City,
                    Country = c.Country,
                    Population = c.Population,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude
                }).ToList() ?? new List<CityDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar ciudades en GeoDb");
                throw;
            }
            

        }

        // -------- 3.2. Buscar ciudades filtrando por pais, poblacion minima o region ----------

        public async Task<List<CityDto>> FilterCitiesAsync(string paisPrefix ,int poblacionMin, string regionPrefix)
        {
            if (paisPrefix.IsNullOrEmpty() && poblacionMin <= 0 && regionPrefix.IsNullOrEmpty())
            {
                return new List<CityDto>(); // Retorna una lista vacia
            }
            try
            {
                var queryParams = new List<string>();

                // Si viene pais, lo agregamos (ej: AR, US, ES...)
                if (!paisPrefix.IsNullOrWhiteSpace())
                {
                    // GeoDB usa countryIds para filtrar por pais (codigos ISO)
                    queryParams.Add($"countryIds={Uri.EscapeDataString(paisPrefix)}");
                }

                //Si viene poblacio mínima > 0, la agregamos
                if (poblacionMin > 0)
                {
                    queryParams.Add($"minPopulation={poblacionMin}");
                }
                
                if (!regionPrefix.IsNullOrWhiteSpace())
                {
                    queryParams.Add($"namePrefix={Uri.EscapeDataString(regionPrefix)}");
                }


                // Limitamos la cantidad de resultados
                queryParams.Add("limit=5");

                //var queryString = string.Join("&", queryParams);
                var url = $"{BaseUrl}/cities?{string.Join("&",queryParams)}";

                _logger.LogWarning("URL enviada a GeoDb (filtros): {Url}", url);

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("X-RapidAPI-Key", ApiKey);
                request.Headers.Add("X-RapidAPI-Host", "wft-geo-db.p.rapidapi.com");

                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new List<CityDto>(); // Retorna lista vacía en lugar de lanzar excepción
                }

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error al consultar GeoDb (filtros): {response.StatusCode}");

                var json = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<GeoDbResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data.Select(c => new CityDto
                {
                    Id = c.Id,
                    Name = c.City,
                    Country = c.Country,
                    Population = c.Population,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude

                }).ToList() ?? new List<CityDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al filtrar ciudades en GeoDb");
                throw;
            }

        }

        // ------------ 3.3 Obtener informacion detallada de una ciudad ---------------
        public async Task<CityDetailDto> GetCityDetailsAsync(int cityId)
        {
            if (cityId <= 0 )
                throw new AbpValidationException("El ID debe ser mayor a cero.");

   
            try
            {
                var url = $"{BaseUrl}/cities/{cityId}";

                _logger.LogWarning("URL enviada a GeoDb (detalle): {Url}", url);

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("X-RapidAPI-Key", ApiKey);
                request.Headers.Add("X-RapidAPI-Host", "wft-geo-db.p.rapidapi.com");

                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new EntityNotFoundException($"No se encontró la ciudad con Id = {cityId}");
                }

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error al consultar GeoDb (detalle): {response.StatusCode}");

                var json = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<GeoDbDetailResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result?.Data == null)
                    throw new EntityNotFoundException($"No se encontró la ciudad con Id = {cityId}");
                

                var c = result.Data;

                return new CityDetailDto
                {
                    Id = c.Id,
                    Name = c.City,
                    Country = c.Country,
                    CountryCode = c.CountryCode,
                    Region = c.Region,
                    RegionCode = c.RegionCode,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude,            
                    Population = c.Population,
                    WikiDataId = c.WikiDataId,
                    TimeZone = c.TimeZone,
                    ElevationMeters = c.ElevationMeters,
                    Type = c.Type
                };
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error al obtener detalle de ciudad en GeoDb");
                throw;
            }
            
        }

        private class GeoDbDetailResponse
        {
            public CityData Data {  get; set; }
        }



            private class GeoDbResponse
            {
                public List<CityData> Data { get; set; }
            }

            private class CityData
            {
                public int Id { get; set; }    
                public string City { get; set; }
                public string Country { get; set; }
                public string CountryCode { get; set; }
                public string Region { get; set; }
                public string RegionCode { get; set; }
                public int Population { get; set; }
                public double Latitude { get; set; }
                public double Longitude { get; set; }
                public string WikiDataId { get; set; }
                public string TimeZone {  get; set; }
                public int? ElevationMeters { get; set; }
                public string Type { get; set; }                
            }
    }
}
