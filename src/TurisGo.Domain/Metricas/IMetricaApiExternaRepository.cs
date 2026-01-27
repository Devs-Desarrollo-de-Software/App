using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace TurisGo.Metricas
{
    public interface IMetricaApiExternaRepository : IRepository<MetricaApiExterna, Guid>
    {
        // Obtiene métricas por nombre de API
        Task<List<MetricaApiExterna>> ObtenerPorApiAsync(
            string nombreApi,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            CancellationToken cancellationToken = default
        );


        // Obtiene estadísticas agregadas de una API
        Task<EstadisticasApiDto> ObtenerEstadisticasAsync(
            string nombreApi,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            CancellationToken cancellationToken = default
        );


        // Obtiene métricas fallidas
        Task<List<MetricaApiExterna>> ObtenerMetricasFallidasAsync(
            string nombreApi = null,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            CancellationToken cancellationToken = default
        );
    }


    // DTO para estadísticas agregadas
    public class EstadisticasApiDto
    {
        public long TotalLlamadas { get; set; }
        public long LlamadasExitosas { get; set; }
        public long LlamadasFallidas { get; set; }
        public double TasaExito { get; set; }
        public double TiempoPromedioMs { get; set; }
        public long TiempoMinimoMs { get; set; }
        public long TiempoMaximoMs { get; set; }
        public int? TotalResultadosDevueltos { get; set; }
    }
}
