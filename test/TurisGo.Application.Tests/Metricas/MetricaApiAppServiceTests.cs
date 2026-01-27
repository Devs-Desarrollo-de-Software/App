using Shouldly;
using System;
using System.Threading.Tasks;
using TurisGo.Metricas;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace TurisGo.Metricas
{
    public abstract class MetricaApiAppServiceTests<TStartupModule> : TurisGoApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IMetricaApiAppService _metricaAppService;
        private readonly IRepository<MetricaApiExterna, Guid> _metricaRepository;

        public MetricaApiAppServiceTests()
        {
            _metricaAppService = GetRequiredService<IMetricaApiAppService>();
            _metricaRepository = GetRequiredService<IRepository<MetricaApiExterna, Guid>>();
        }

        [Fact]
        public async Task ObtenerMetricasAsync_DebeRetornarMetricas_CuandoExisten()
        {
            // Arrange
            await CrearMetricaDePruebaAsync("GeoDB");
            await CrearMetricaDePruebaAsync("GeoDB");

            var input = new ObtenerMetricasInput
            {
                NombreApi = "GeoDB",
                MaxResultCount = 10
            };

            // Act
            var result = await _metricaAppService.ObtenerMetricasAsync(input);

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);
            result[0].NombreApi.ShouldBe("GeoDB");
        }

        [Fact]
        public async Task ObtenerEstadisticasAsync_DebeCalcularCorrectamente()
        {
            // Arrange
            await CrearMetricaDePruebaAsync("TicketMaster", exitosa: true, tiempoMs: 100);
            await CrearMetricaDePruebaAsync("TicketMaster", exitosa: true, tiempoMs: 200);
            await CrearMetricaDePruebaAsync("TicketMaster", exitosa: false, tiempoMs: 500);

            // Act
            var result = await _metricaAppService.ObtenerEstadisticasAsync("TicketMaster");

            // Assert
            result.ShouldNotBeNull();
            result.NombreApi.ShouldBe("TicketMaster");
            result.TotalLlamadas.ShouldBeGreaterThanOrEqualTo(3);
            result.LlamadasExitosas.ShouldBeGreaterThanOrEqualTo(2);
            result.LlamadasFallidas.ShouldBeGreaterThanOrEqualTo(1);
            result.TasaExito.ShouldBeGreaterThan(0);
            result.TasaExito.ShouldBeLessThan(100);
        }

        [Fact]
        public async Task ObtenerMetricasFallidasAsync_DebeFiltrarCorrectamente()
        {
            // Arrange
            await CrearMetricaDePruebaAsync("TestAPI", exitosa: true);
            await CrearMetricaDePruebaAsync("TestAPI", exitosa: false);
            await CrearMetricaDePruebaAsync("TestAPI", exitosa: false);

            // Act
            var result = await _metricaAppService.ObtenerMetricasFallidasAsync("TestAPI");

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThanOrEqualTo(2);
            result.ShouldAllBe(m => !m.Exitosa);
        }

        private async Task<MetricaApiExterna> CrearMetricaDePruebaAsync(
            string nombreApi,
            bool exitosa = true,
            long tiempoMs = 150)
        {
            var metrica = new MetricaApiExterna(
                Guid.NewGuid(),
                nombreApi,
                "/test/endpoint",
                "GET",
                exitosa ? 200 : 500,
                tiempoMs,
                exitosa
            );

            if (!exitosa)
            {
                metrica.SetError("Error de prueba");
            }

            metrica.SetCantidadResultados(5);

            return await _metricaRepository.InsertAsync(metrica, autoSave: true);
        }
    }
}
