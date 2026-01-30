using System;
using Volo.Abp.Application.Dtos;

namespace TurisGo.Metricas
{
    public class MetricaApiDto : EntityDto<Guid>
    {
        public string NombreApi { get; set; }
        public string Endpoint { get; set; }
        public string MetodoHttp { get; set; }
        public string ParametrosConsulta { get; set; }
        public int CodigoEstadoHttp { get; set; }
        public long TiempoRespuestaMs { get; set; }
        public bool Exitosa { get; set; }
        public string MensajeError { get; set; }
        public int? CantidadResultados { get; set; }
        public DateTime FechaHora { get; set; }
    }

    public class EstadisticasApiExternaDto
    {
        public string NombreApi { get; set; }
        public long TotalLlamadas { get; set; }
        public long LlamadasExitosas { get; set; }
        public long LlamadasFallidas { get; set; }
        public double TasaExito { get; set; }
        public double TiempoPromedioMs { get; set; }
        public long TiempoMinimoMs { get; set; }
        public long TiempoMaximoMs { get; set; }
        public int? TotalResultadosDevueltos { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }

    public class ObtenerMetricasInput
    {
        public string NombreApi { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public bool SoloFallidas { get; set; } = false;
        public int MaxResultCount { get; set; } = 100;
    }
}
