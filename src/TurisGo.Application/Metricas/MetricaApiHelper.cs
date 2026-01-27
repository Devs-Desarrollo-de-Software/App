using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace TurisGo.Metricas
{
    /// <summary>
    /// Helper para registrar métricas de llamadas a APIs externas
    /// </summary>
    public class MetricaApiHelper : ITransientDependency
    {
        private readonly IRepository<MetricaApiExterna, Guid> _metricaRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ILogger<MetricaApiHelper> _logger;

        public MetricaApiHelper(
            IRepository<MetricaApiExterna, Guid> metricaRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ILogger<MetricaApiHelper> logger)
        {
            _metricaRepository = metricaRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _logger = logger;
        }

        /// <summary>
        /// Ejecuta una llamada a API externa y registra las métricas
        /// </summary>
        public async Task<T> EjecutarYRegistrarAsync<T>(
            string nombreApi,
            string endpoint,
            string metodoHttp,
            object parametros,
            Func<Task<T>> accion)
        {
            var stopwatch = Stopwatch.StartNew();
            MetricaApiExterna metrica = null;
            T resultado = default;
            bool exitosa = false;
            int codigoEstado = 0;
            string mensajeError = null;
            int? cantidadResultados = null;

            try
            {
                resultado = await accion();
                exitosa = true;
                codigoEstado = 200; // Asumimos éxito

                // Intentar obtener cantidad de resultados si es una lista
                if (resultado is System.Collections.IEnumerable enumerable && resultado is not string)
                {
                    var count = 0;
                    foreach (var item in enumerable)
                    {
                        count++;
                    }
                    cantidadResultados = count;
                }

                return resultado;
            }
            catch (Exception ex)
            {
                exitosa = false;
                mensajeError = ex.Message;
                codigoEstado = ex is System.Net.Http.HttpRequestException ? 500 : 0;
                throw;
            }
            finally
            {
                stopwatch.Stop();

                // Registrar métrica en una UoW separada para que se guarde incluso si hay error
                try
                {
                    using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
                    {
                        metrica = new MetricaApiExterna(
                            Guid.NewGuid(),
                            nombreApi,
                            endpoint,
                            metodoHttp,
                            codigoEstado,
                            stopwatch.ElapsedMilliseconds,
                            exitosa
                        );

                        if (parametros != null)
                        {
                            var parametrosJson = JsonSerializer.Serialize(parametros, new JsonSerializerOptions
                            {
                                WriteIndented = false,
                                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                            });
                            metrica.SetParametrosConsulta(parametrosJson);
                        }

                        if (!string.IsNullOrWhiteSpace(mensajeError))
                        {
                            metrica.SetError(mensajeError);
                        }

                        if (cantidadResultados.HasValue)
                        {
                            metrica.SetCantidadResultados(cantidadResultados.Value);
                        }

                        await _metricaRepository.InsertAsync(metrica, autoSave: true);
                        await uow.CompleteAsync();
                        _logger.LogInformation("Métrica registrada: {NombreApi} - {Endpoint} - Exitosa: {Exitosa} - Tiempo: {TiempoMs}ms",
                            nombreApi, endpoint, exitosa, stopwatch.ElapsedMilliseconds);
                    }
                }
                catch (Exception ex)
                {
                    // Si falla el registro de métrica, no queremos afectar la operación principal
                    // Pero lo logueamos para diagnóstico
                    _logger.LogError(ex, "Error al registrar métrica de API: {NombreApi} - {Endpoint}", nombreApi, endpoint);
                }
            }
        }
    }
}
