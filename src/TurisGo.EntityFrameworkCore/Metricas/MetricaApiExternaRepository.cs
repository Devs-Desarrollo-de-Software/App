using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TurisGo.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace TurisGo.Metricas
{
    public class MetricaApiExternaRepository : EfCoreRepository<TurisGoDbContext, MetricaApiExterna, Guid>, IMetricaApiExternaRepository
    {
        public MetricaApiExternaRepository(IDbContextProvider<TurisGoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<MetricaApiExterna>> ObtenerPorApiAsync(
            string nombreApi,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            CancellationToken cancellationToken = default)
        {
            var query = (await GetDbSetAsync())
                .Where(m => m.NombreApi == nombreApi);

            if (fechaDesde.HasValue)
            {
                query = query.Where(m => m.CreationTime >= fechaDesde.Value);
            }

            if (fechaHasta.HasValue)
            {
                query = query.Where(m => m.CreationTime <= fechaHasta.Value);
            }

            return await query
                .OrderByDescending(m => m.CreationTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<EstadisticasApiDto> ObtenerEstadisticasAsync(
            string nombreApi,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            CancellationToken cancellationToken = default)
        {
            var query = (await GetDbSetAsync()).AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombreApi))
            {
                query = query.Where(m => m.NombreApi == nombreApi);
            }

            if (fechaDesde.HasValue)
            {
                query = query.Where(m => m.CreationTime >= fechaDesde.Value);
            }

            if (fechaHasta.HasValue)
            {
                query = query.Where(m => m.CreationTime <= fechaHasta.Value);
            }

            var metricas = await query.ToListAsync(cancellationToken);

            if (!metricas.Any())
            {
                return new EstadisticasApiDto();
            }

            var totalLlamadas = metricas.Count;
            var llamadasExitosas = metricas.Count(m => m.Exitosa);
            var llamadasFallidas = totalLlamadas - llamadasExitosas;

            return new EstadisticasApiDto
            {
                TotalLlamadas = totalLlamadas,
                LlamadasExitosas = llamadasExitosas,
                LlamadasFallidas = llamadasFallidas,
                TasaExito = totalLlamadas > 0 ? (double)llamadasExitosas / totalLlamadas * 100 : 0,
                TiempoPromedioMs = metricas.Average(m => m.TiempoRespuestaMs),
                TiempoMinimoMs = metricas.Min(m => m.TiempoRespuestaMs),
                TiempoMaximoMs = metricas.Max(m => m.TiempoRespuestaMs),
                TotalResultadosDevueltos = metricas.Where(m => m.CantidadResultados.HasValue).Sum(m => m.CantidadResultados)
            };
        }

        public async Task<List<MetricaApiExterna>> ObtenerMetricasFallidasAsync(
            string nombreApi = null,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            CancellationToken cancellationToken = default)
        {
            var query = (await GetDbSetAsync())
                .Where(m => !m.Exitosa);

            if (!string.IsNullOrWhiteSpace(nombreApi))
            {
                query = query.Where(m => m.NombreApi == nombreApi);
            }

            if (fechaDesde.HasValue)
            {
                query = query.Where(m => m.CreationTime >= fechaDesde.Value);
            }

            if (fechaHasta.HasValue)
            {
                query = query.Where(m => m.CreationTime <= fechaHasta.Value);
            }

            return await query
                .OrderByDescending(m => m.CreationTime)
                .ToListAsync(cancellationToken);
        }
    }
}
