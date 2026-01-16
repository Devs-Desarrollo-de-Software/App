using Microsoft.Extensions.Caching.Memory;
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
        private readonly Microsoft.Extensions.Caching.Memory.IMemoryCache _cache; // Inject Cache
        
        private const string ApiKey = "9332dd950amsh49b8d385752ec23p13d526jsn5974666e3749";// Replace with your actual API key
        private const string BaseUrl = "https://wft-geo-db.p.rapidapi.com/v1/geo";
        

        public GeoDbCitySearchService(HttpClient httpClient, ILogger<GeoDbCitySearchService> logger, Microsoft.Extensions.Caching.Memory.IMemoryCache cache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _cache = cache;
        }

        // --------------- 3.1 Buscar ciudades por nombre --------------------------
        // Operación del Sistema: BÚSQUEDA SIMPLE
        // Este método se encarga de buscar ciudades que coincidan con el prefijo dado.
        // Es la operación utilizada por el autocompletado del buscador principal.
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
        // Cambiamos la firma para aceptar tambien 'cityNamePrefix' si se desea filtrar por nombre ADEMAS de los otros filtros
        // Operación del Sistema: FILTRADO AVANZADO
        // Este método implementa la lógica principal de filtros (País, Región, Población)
        // Se encarga de orquestar la resolución de códigos ISO y llamar al endpoint correcto de GeoDB.
        public async Task<List<CityDto>> FilterCitiesAsync(string paisPrefix ,int poblacionMin, string regionPrefix, string cityNamePrefix = null)
        {
            // Ya no retornamos lista vacía si no hay filtros.
            // Si todo es null, el flujo continuará y construirá una query a /cities?sort=-population&limit=10
            // resultando en "Destinos Populares".
            
            try
            {
                string countryCode = null;
                string countryName = null;
                string regionCode = null;

                // 1. Resolver PAIS (con Cache)
                if (!paisPrefix.IsNullOrWhiteSpace())
                {
                    string countryInput = paisPrefix.Trim();
                    
                    // Clave de cache única para este input
                    string cacheKey = $"Country_Data_{countryInput.ToLower()}";
                    
                    if (!_cache.TryGetValue(cacheKey, out CountryData cachedCountry))
                    {
                         // Si es codigo (len=2), podriamos intentar usarlo directo, pero para obtener el NOMBRE correcto
                         // vamos a intentar resolverlo igual si queremos mostrar "United States" en vez de "US".
                         // O, para ser eficientes, si es len=2 asumimos que es el codigo y el nombre es el mismo codigo por ahora,
                         // salvo que llamemos a la API para detalles. 
                         // Para mantenerlo simple: Si len > 2 llamamos API. Si len=2 usamos directo.
                         
                         if (countryInput.Length == 2) 
                         {
                             // Intentamos resolver el nombre real usando el código
                             var resolved = await GetCountryDataByCodeAsync(countryInput.ToUpper());
                             if (resolved != null)
                             {
                                 countryCode = resolved.Code;
                                 countryName = resolved.Name;
                                 // Cache
                                 _cache.Set(cacheKey, resolved, TimeSpan.FromDays(1));
                             }
                             else
                             {
                                 // Fallback
                                 countryCode = countryInput.ToUpper();
                                 countryName = countryCode;
                             }
                         }
                         else
                         {
                            var resolved = await GetCountryDataByNameAsync(countryInput);
                             if (resolved != null)
                             {
                                 countryCode = resolved.Code;
                                 countryName = resolved.Name;
                                 
                                 // Guardamos en cache el objeto completo
                                 _cache.Set(cacheKey, resolved, TimeSpan.FromDays(1));
                             }
                         }
                    }
                    else
                    {
                        countryCode = cachedCountry.Code;
                        countryName = cachedCountry.Name;
                    }

                    if (string.IsNullOrEmpty(countryCode))
                    {
                        _logger.LogWarning("No se encontró código ISO para el país: {Pais}", paisPrefix);
                        return new List<CityDto>();
                    }
                }

                // 2. Resolver REGION (con Cache)
                if (!string.IsNullOrEmpty(countryCode) && !regionPrefix.IsNullOrWhiteSpace())
                {
                    string regionInput = regionPrefix.Trim();
                    string cacheKey = $"Region_ISO_{countryCode}_{regionInput.ToLower()}";
                    
                    if (!_cache.TryGetValue(cacheKey, out regionCode))
                    {
                         // Agregar pequeño delay para evitar 429 si se llama muy seguido del anterior
                         await Task.Delay(500); 

                         regionCode = await GetRegionIsoCodeByNameAsync(countryCode, regionInput);
                         if (regionCode != null)
                         {
                             _cache.Set(cacheKey, regionCode, TimeSpan.FromDays(1));
                         }
                    }
                    
                    if (string.IsNullOrEmpty(regionCode))
                    {
                         _logger.LogWarning("No se encontró código de región '{Reg}' en país '{Pais}'", regionPrefix, countryCode);
                         return new List<CityDto>();
                    }
                }


                // 3. Construir URL base según jerarquía
                // Jerarquía: /countries/{id}/regions/{code}/cities  -> Mejor especificidad
                //      sino: /cities (con params)
                
                string url;
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(countryCode) && !string.IsNullOrEmpty(regionCode))
                {
                    // Endpoint específico de región
                    url = $"{BaseUrl}/countries/{countryCode}/regions/{regionCode}/cities";
                }
                else 
                {
                    // Endpoint genérico
                    url = $"{BaseUrl}/cities";
                    
                    if (!string.IsNullOrEmpty(countryCode))
                    {
                        queryParams.Add($"countryIds={countryCode}");
                    }
                    
                    // Si tenemos región pero no país... GeoDB no soporta "region name" global muy bien.
                    // En este caso, si no hay país, ignoramos regionPrefix lamentablemente.
                }

                // Parametros comunes
                if (poblacionMin > 0)
                {
                    queryParams.Add($"minPopulation={poblacionMin}");
                }

                // AHORA SI: Usamos el nombre de la ciudad como filtro en la query (namePrefix funciona en ambos endpoints)
                if (!cityNamePrefix.IsNullOrWhiteSpace())
                {
                    queryParams.Add($"namePrefix={Uri.EscapeDataString(cityNamePrefix)}");
                }

                queryParams.Add("limit=10"); // Aumentamos limit a 10 para mejor UX
                queryParams.Add("sort=-population");

                if (queryParams.Any())
                {
                    url += "?" + string.Join("&", queryParams);
                }

                _logger.LogWarning("URL enviada a GeoDb (filtros): {Url}", url);

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("X-RapidAPI-Key", ApiKey);
                request.Headers.Add("X-RapidAPI-Host", "wft-geo-db.p.rapidapi.com");

                // Check 429
                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                     // Si fallamos por rate limit, esperamos 1.5 seg y reintentamos una vez
                     _logger.LogWarning("Rate Limit alcanzado (429). Esperando para reintentar...");
                     await Task.Delay(1500); 
                     
                     using var retryRequest = new HttpRequestMessage(HttpMethod.Get, url);
                     retryRequest.Headers.Add("X-RapidAPI-Key", ApiKey);
                     retryRequest.Headers.Add("X-RapidAPI-Host", "wft-geo-db.p.rapidapi.com");
                     response = await _httpClient.SendAsync(retryRequest);
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return new List<CityDto>();

                if (!response.IsSuccessStatusCode)
                {
                     throw new Exception($"Error al consultar GeoDb (filtros): {response.StatusCode}");
                }

                var json = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<GeoDbResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data.Select(c => new CityDto
                {
                    Id = c.Id,
                    Name = c.City,
                    // Si la API devuelve el pais vacio (pasa en endpoints jerarquicos), usamos el que resolvimos nosotros
                    Country = !string.IsNullOrEmpty(c.Country) ? c.Country : (countryName ?? countryCode),
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

        // Helper: Resolución de Nombres
        // Convierte un nombre de país (ej. "Argentina") en su código ISO (ej. "AR") necesario para la API.
        private async Task<CountryData> GetCountryDataByNameAsync(string countryName)
        {
            try 
            {
                // Endpoint para buscar paises por nombre
                var url = $"{BaseUrl}/countries?namePrefix={Uri.EscapeDataString(countryName)}&limit=1";
                
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("X-RapidAPI-Key", ApiKey);
                request.Headers.Add("X-RapidAPI-Host", "wft-geo-db.p.rapidapi.com");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GeoDbCountryResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return result?.Data?.FirstOrDefault(); // Retorna el objeto CountryData completo
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error resolviendo país {Pais}", countryName);
                return null;
            }
        }


        // Helper: Resolución de Regiones
        // Busca el código ISO de una región dentro de un país específico.
        private async Task<string> GetRegionIsoCodeByNameAsync(string countryCode, string regionName)
        {
            try
            {
                // Buscamos regiones DENTRO del país
                var url = $"{BaseUrl}/countries/{countryCode}/regions?namePrefix={Uri.EscapeDataString(regionName)}&limit=1";
                
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("X-RapidAPI-Key", ApiKey);
                request.Headers.Add("X-RapidAPI-Host", "wft-geo-db.p.rapidapi.com");

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GeoDbRegionResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Data?.FirstOrDefault()?.IsoCode; // GeoDB retorna 'isoCode' o 'fipsCode'
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error resolviendo región {Reg} en {Pais}", regionName, countryCode);
                 return null;
            }
        }

        // ------------ 3.3 Obtener informacion detallada de una ciudad ---------------
        // Operación del Sistema: DETALLE DE CIUDAD
        // Obtiene todos los datos detallados de una ciudad específica por su ID.
        // Utilizado para mostrar la vista previa o guardar el destino.
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

        private class GeoDbCountryResponse
        {
            public List<CountryData> Data { get; set; }
        }

        private class CountryData
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }


        private class GeoDbRegionResponse
        {
            public List<RegionData> Data { get; set; }
        }

        private class RegionData
        {
            public string IsoCode { get; set; }
            public string Name { get; set; }
        }

        private class GeoDbResponse
        {
            public List<CityData> Data { get; set; }
        }


        // Helper: Resolución Inversa
        // Obtiene el nombre completo de un país dado su código ISO (ej. "AR" -> "Argentina").
        private async Task<CountryData> GetCountryDataByCodeAsync(string countryCode)
        {
            try 
            {
                var url = $"{BaseUrl}/countries/{countryCode}"; 
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("X-RapidAPI-Key", ApiKey);
                request.Headers.Add("X-RapidAPI-Host", "wft-geo-db.p.rapidapi.com");

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                
                // Usamos JsonDocument para extraer data.code y data.name sin crear DTO especifico
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("data", out var data))
                {
                     return new CountryData 
                     { 
                         Code = data.GetProperty("code").GetString() ?? countryCode, 
                         Name = data.GetProperty("name").GetString() ?? countryCode 
                     };
                }
                return null;
            }
            catch(Exception ex)
            {
                 _logger.LogError(ex, "Error resolviendo país por código {Code}", countryCode);
                 return null;
            }
        }


        // Operación del Sistema: DESTINOS POPULARES
        // Obtiene las ciudades con mayor población (Top 10 por defecto).
        public async Task<List<CityDto>> GetPopularCitiesAsync(int limit = 10)
        {
             try
            {
                var url = $"{BaseUrl}/cities?sort=-population&limit={limit}";

                 _logger.LogWarning("URL enviada a GeoDb (populares): {Url}", url);

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("X-RapidAPI-Key", ApiKey);
                request.Headers.Add("X-RapidAPI-Host", "wft-geo-db.p.rapidapi.com");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error al consultar GeoDb (populares): {response.StatusCode}");

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
             catch(Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo ciudades populares");
                throw;
            }
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
