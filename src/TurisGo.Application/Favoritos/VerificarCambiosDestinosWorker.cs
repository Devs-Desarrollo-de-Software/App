using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Threading;
using Volo.Abp.Uow;
using TurisGo.Destinos;
using TurisGo.Notificaciones;

namespace TurisGo.Favoritos
{
    // Background Worker que verifica cambios en destinos favoritos diariamente
    // Implementa el requisito: "El sistema verificará periódicamente (ej: una vez por día)"
    // Genera notificaciones cuando hay cambios relevantes en los destinos favoritos del usuario
    public class VerificarCambiosDestinosWorker : AsyncPeriodicBackgroundWorkerBase
    {
        public VerificarCambiosDestinosWorker(
            AbpAsyncTimer timer,
            IServiceScopeFactory serviceScopeFactory)
            : base(timer, serviceScopeFactory)
        {
            // Ejecutar cada 24 horas (1 vez al día a las 2 AM)
            Timer.Period = 24 * 60 * 60 * 1000;
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var ahora = DateTime.Now;

            // Solo ejecutar a las 2 AM
            if (ahora.Hour != 2)
            {
                return;
            }

            var logger = workerContext.ServiceProvider
                .GetRequiredService<ILogger<VerificarCambiosDestinosWorker>>();

            logger.LogInformation("Iniciando verificación diaria de cambios en destinos favoritos...");

            try
            {
                await VerificarCambiosAsync(workerContext.ServiceProvider);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en verificación de cambios de destinos");
            }
        }

        private async Task VerificarCambiosAsync(IServiceProvider serviceProvider)
        {
            var unitOfWorkManager = serviceProvider.GetRequiredService<IUnitOfWorkManager>();
            var favoritoRepository = serviceProvider.GetRequiredService<IRepository<Favorito, Guid>>();
            var destinoRepository = serviceProvider.GetRequiredService<IRepository<Destino, Guid>>();
            var notificacionRepository = serviceProvider.GetRequiredService<IRepository<Notificacion, Guid>>();
            var citySearchService = serviceProvider.GetRequiredService<ICitySearchService>();
            var logger = serviceProvider.GetRequiredService<ILogger<VerificarCambiosDestinosWorker>>();

            using (var uow = unitOfWorkManager.Begin(requiresNew: true))
            {
                // Obtener todos los destinos que tienen favoritos
                var queryable = await favoritoRepository.GetQueryableAsync();
                var favoritosAgrupados = queryable
                    .GroupBy(f => f.DestinoId)
                    .Select(g => new { DestinoId = g.Key, Usuarios = g.Select(f => f.UserId).ToList() })
                    .ToList();

                logger.LogInformation(
                    "Verificando {Count} destinos con favoritos",
                    favoritosAgrupados.Count);

                foreach (var grupo in favoritosAgrupados)
                {
                    try
                    {
                        var destino = await destinoRepository.GetAsync(grupo.DestinoId);
                        var cambiosDetectados = new List<string>();

                        // 1. Verificar cambios en datos del destino (población, etc.)
                        // Nota: GeoDB Cities API no siempre actualiza población frecuentemente
                        // Este es un ejemplo de verificación básica
                        // En el futuro se pueden agregar más verificaciones según las necesidades

                        // Si hay cambios, notificar a todos los usuarios que tienen este destino en favoritos
                        if (cambiosDetectados.Any())
                        {
                            logger.LogInformation(
                                "Cambios detectados en {Destino}: {Cambios}",
                                destino.Nombre,
                                string.Join("; ", cambiosDetectados));

                            foreach (var userId in grupo.Usuarios)
                            {
                                var notificacion = new Notificacion(
                                     Guid.NewGuid(),
                                     userId,
                                     destino.Id,
                                     "Novedades en tu destino favorito",          // titulo
                                     string.Join(". ", cambiosDetectados),        // mensaje
                                     TipoNotificacion.ActualizacionDatos,         // tipo
                                     destino.Nombre                               // nombreDestino
                                );

                                await notificacionRepository.InsertAsync(notificacion);
                            }

                            logger.LogInformation(
                                "Notificaciones creadas para {Count} usuarios sobre {Destino}",
                                grupo.Usuarios.Count,
                                destino.Nombre);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex,
                            "Error al verificar destino {DestinoId}",
                            grupo.DestinoId);
                    }
                }

                await uow.CompleteAsync();
                logger.LogInformation("Verificación de cambios completada");
            }
        }
    }
}
