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
using TurisGo.Usuarios;

namespace TurisGo.Notificaciones
{
    /// <summary>
    /// Background Worker que envía resúmenes semanales cada domingo a las 20:00
    /// </summary>
    public class EnviarResumenSemanalWorker : AsyncPeriodicBackgroundWorkerBase
    {
        public EnviarResumenSemanalWorker(
            AbpAsyncTimer timer,
            IServiceScopeFactory serviceScopeFactory)
            : base(timer, serviceScopeFactory)
        {
            // Ejecutar cada 1 hora (verificar si es domingo)
            Timer.Period = 60 * 60 * 1000; // 1 hora
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            // Solo ejecutar los domingos entre las 20:00 y 21:00
            var ahora = DateTime.Now;
            if (ahora.DayOfWeek != DayOfWeek.Sunday || ahora.Hour != 20)
            {
                return;
            }

            var logger = workerContext.ServiceProvider
                .GetRequiredService<ILogger<EnviarResumenSemanalWorker>>();

            logger.LogInformation("Iniciando envío de resúmenes semanales...");

            try
            {
                await EnviarResumenesSemanalesAsync(workerContext.ServiceProvider);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al enviar resúmenes semanales");
            }
        }

        private async Task EnviarResumenesSemanalesAsync(IServiceProvider serviceProvider)
        {
            var unitOfWorkManager = serviceProvider.GetRequiredService<IUnitOfWorkManager>();
            var notificacionRepository = serviceProvider.GetRequiredService<IRepository<Notificacion, Guid>>();
            var usuarioRepository = serviceProvider.GetRequiredService<IRepository<Usuario, Guid>>();
            var emailService = serviceProvider.GetRequiredService<EmailService>();
            var logger = serviceProvider.GetRequiredService<ILogger<EnviarResumenSemanalWorker>>();

            using (var uow = unitOfWorkManager.Begin(requiresNew: true))
            {
                // Obtener usuarios con preferencia semanal
                var queryableUsuarios = await usuarioRepository.GetQueryableAsync();
                var usuariosSemanales = queryableUsuarios
                    .Where(u => u.Preferencias.RecibirPorEmail &&
                               u.Preferencias.Frecuencia == FrecuenciaNotificacion.Semanal)
                    .ToList();

                logger.LogInformation("Procesando resumen semanal para {Count} usuarios", usuariosSemanales.Count);

                var hace7Dias = DateTime.UtcNow.AddDays(-7);

                foreach (var usuario in usuariosSemanales)
                {
                    try
                    {
                        // Obtener notificaciones de la última semana no enviadas
                        var queryableNotif = await notificacionRepository.GetQueryableAsync();
                        var notificacionesSemana = queryableNotif
                            .Where(n => n.UserId == usuario.Id &&
                                       !n.EnviadaPorMail &&
                                       n.CreationTime >= hace7Dias)
                            .OrderByDescending(n => n.CreationTime)
                            .ToList();

                        if (!notificacionesSemana.Any())
                        {
                            logger.LogInformation(
                                "Usuario {Email} no tiene notificaciones pendientes esta semana",
                                usuario.Email);
                            continue;
                        }

                        // Preparar datos para el resumen
                        var resumen = notificacionesSemana.Select(n => new NotificacionResumenDto
                        {
                            NombreDestino = n.NombreDestino,
                            Titulo = n.Titulo,
                            Mensaje = n.Mensaje,
                            Fecha = n.CreationTime
                        }).ToList();

                        // Enviar resumen
                        await emailService.EnviarResumenSemanalAsync(usuario.Email, resumen);

                        // Marcar notificaciones como enviadas
                        foreach (var notif in notificacionesSemana)
                        {
                            notif.MarcarComoEnviadaPorMail();
                        }

                        logger.LogInformation(
                            "Resumen semanal enviado a {Email} con {Count} notificaciones",
                            usuario.Email,
                            notificacionesSemana.Count);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex,
                            "Error al enviar resumen semanal a {Email}",
                            usuario.Email);
                    }
                }

                await uow.CompleteAsync();
                logger.LogInformation("Envío de resúmenes semanales completado");
            }
        }
    }
}
