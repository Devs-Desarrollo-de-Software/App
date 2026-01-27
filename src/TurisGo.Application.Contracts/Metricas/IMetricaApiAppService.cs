using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TurisGo.Metricas
{
    public interface IMetricaApiAppService : IApplicationService
    {
        /// <summary>
        /// 8.1. Consultar métricas de uso de la API externa
        /// </summary>
        Task<List<MetricaApiDto>> ObtenerMetricasAsync(ObtenerMetricasInput input);

        /// <summary>
        /// Obtiene estadísticas agregadas de una API
        /// </summary>
        Task<EstadisticasApiExternaDto> ObtenerEstadisticasAsync(string nombreApi, System.DateTime? fechaDesde = null, System.DateTime? fechaHasta = null);

        /// <summary>
        /// Obtiene métricas fallidas (para diagnóstico)
        /// </summary>
        Task<List<MetricaApiDto>> ObtenerMetricasFallidasAsync(string nombreApi = null, System.DateTime? fechaDesde = null, System.DateTime? fechaHasta = null);
    }
}
