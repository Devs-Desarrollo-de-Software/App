using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Services;

namespace TurisGo.Metricas
{
    [Authorize(Roles = "admin")]
    public class MetricaApiAppService : ApplicationService, IMetricaApiAppService
    {
        private readonly IMetricaApiExternaRepository _metricaRepository;

        public MetricaApiAppService(IMetricaApiExternaRepository metricaRepository)
        {
            _metricaRepository = metricaRepository;
        }

        /// <summary>
        /// 8.1. Consultar métricas de uso de la API externa
        /// </summary>
        [HttpGet]
        [Route("api/app/metrica-api/metricas")]
        public async Task<List<MetricaApiDto>> ObtenerMetricasAsync([FromQuery] ObtenerMetricasInput input)
        {
            List<MetricaApiExterna> metricas;

            if (input.SoloFallidas)
            {
                metricas = await _metricaRepository.ObtenerMetricasFallidasAsync(
                    input.NombreApi,
                    input.FechaDesde,
                    input.FechaHasta
                );
            }
            else if (!string.IsNullOrWhiteSpace(input.NombreApi))
            {
                metricas = await _metricaRepository.ObtenerPorApiAsync(
                    input.NombreApi,
                    input.FechaDesde,
                    input.FechaHasta
                );
            }
            else
            {
                var queryable = await _metricaRepository.GetQueryableAsync();
                var query = queryable.AsQueryable();

                if (input.FechaDesde.HasValue)
                {
                    query = query.Where(m => m.CreationTime >= input.FechaDesde.Value);
                }

                if (input.FechaHasta.HasValue)
                {
                    query = query.Where(m => m.CreationTime <= input.FechaHasta.Value);
                }

                metricas = query
                    .OrderByDescending(m => m.CreationTime)
                    .Take(input.MaxResultCount)
                    .ToList();
            }

            return metricas
                .Take(input.MaxResultCount)
                .Select(MapearADto)
                .ToList();
        }

        /// <summary>
        /// Obtiene estadísticas agregadas de una API
        /// </summary>
        [HttpGet]
        [Route("api/app/metrica-api/estadisticas")]
        public async Task<EstadisticasApiExternaDto> ObtenerEstadisticasAsync(
            [FromQuery] string nombreApi,
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null)
        {
            var estadisticas = await _metricaRepository.ObtenerEstadisticasAsync(
                nombreApi,
                fechaDesde,
                fechaHasta
            );

            return new EstadisticasApiExternaDto
            {
                NombreApi = nombreApi,
                TotalLlamadas = estadisticas.TotalLlamadas,
                LlamadasExitosas = estadisticas.LlamadasExitosas,
                LlamadasFallidas = estadisticas.LlamadasFallidas,
                TasaExito = estadisticas.TasaExito,
                TiempoPromedioMs = estadisticas.TiempoPromedioMs,
                TiempoMinimoMs = estadisticas.TiempoMinimoMs,
                TiempoMaximoMs = estadisticas.TiempoMaximoMs,
                TotalResultadosDevueltos = estadisticas.TotalResultadosDevueltos,
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta
            };
        }

        /// <summary>
        /// Obtiene métricas fallidas (para diagnóstico)
        /// </summary>
        [HttpGet]
        [Route("api/app/metrica-api/metricas-fallidas")]
        public async Task<List<MetricaApiDto>> ObtenerMetricasFallidasAsync(
            [FromQuery] string nombreApi = null,
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null)
        {
            var metricas = await _metricaRepository.ObtenerMetricasFallidasAsync(
                nombreApi,
                fechaDesde,
                fechaHasta
            );

            return metricas.Select(MapearADto).ToList();
        }

        private MetricaApiDto MapearADto(MetricaApiExterna metrica)
        {
            return new MetricaApiDto
            {
                Id = metrica.Id,
                NombreApi = metrica.NombreApi,
                Endpoint = metrica.Endpoint,
                MetodoHttp = metrica.MetodoHttp,
                ParametrosConsulta = metrica.ParametrosConsulta,
                CodigoEstadoHttp = metrica.CodigoEstadoHttp,
                TiempoRespuestaMs = metrica.TiempoRespuestaMs,
                Exitosa = metrica.Exitosa,
                MensajeError = metrica.MensajeError,
                CantidadResultados = metrica.CantidadResultados,
                FechaHora = metrica.CreationTime
            };
        }
    }
}
