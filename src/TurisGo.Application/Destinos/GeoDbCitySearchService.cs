using Microsoft.Extensions.Logging;
using Scriban.Runtime.Accessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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

            private class GeoDbResponse
            {
                public List<CityData> Data { get; set; }
            }

            private class CityData
            {
                public string City { get; set; }
                public string Country { get; set; }
                public int Population { get; set; }
                public double Latitude { get; set; }
                public double Longitude { get; set; }
            }
    }
}
