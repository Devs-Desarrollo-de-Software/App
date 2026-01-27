using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace TurisGo.Metricas
{
    // Entidad para registrar métricas de uso de APIs externas (GeoDB, TicketMaster)
    // Permite monitorear rendimiento, errores y uso de las APIs de terceros
    public class MetricaApiExterna : CreationAuditedEntity<Guid>
    {
        public string NombreApi { get; set; }       // Nombre de la API externa (GeoDB, TicketMaster)
        public string Endpoint { get; set; }        // Endpoint llamado en la API
        public string MetodoHttp { get; set; }      // Método HTTP usado (GET, POST, etc.)
        public string? ParametrosConsulta { get; set; }     // Parámetros de la consulta en formato JSON
        public int CodigoEstadoHttp { get; set; }       // Código de estado HTTP de la respuesta
        public long TiempoRespuestaMs { get; set; }     // Tiempo de respuesta en milisegundos
        public bool Exitosa { get; set; }
        public string? MensajeError { get; set; }
        public int? CantidadResultados { get; set; }

        protected MetricaApiExterna()
        {
        }

        public MetricaApiExterna(
            Guid id,
            string nombreApi,
            string endpoint,
            string metodoHttp,
            int codigoEstadoHttp,
            long tiempoRespuestaMs,
            bool exitosa
        ) : base(id)
        {
            NombreApi = nombreApi;
            Endpoint = endpoint;
            MetodoHttp = metodoHttp;
            CodigoEstadoHttp = codigoEstadoHttp;
            TiempoRespuestaMs = tiempoRespuestaMs;
            Exitosa = exitosa;
            MensajeError = string.Empty; // Inicializar vacío para evitar NULL en BD
        }

        public void SetParametrosConsulta(string parametros)
        {
            ParametrosConsulta = parametros;
        }

        public void SetError(string mensajeError)
        {
            MensajeError = mensajeError;
            Exitosa = false;
        }

        public void SetCantidadResultados(int cantidad)
        {
            CantidadResultados = cantidad;
        }
    }
}
