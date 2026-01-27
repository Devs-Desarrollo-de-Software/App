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
    /// Background Worker que envía notificaciones por email cada 5 minutos
    /// 7.2. Notificar sobre cambios relevantes en destinos (por email)
    /// </summary>
    public class EnviarNotificacionesEmailWorker : AsyncPeriodicBackgroundWorkerBase
    {
        public EnviarNotificacionesEmailWorker(
            AbpAsyncTimer timer,
            IServiceScopeFactory serviceScopeFactory)
            : base(timer, serviceScopeFactory)
        {
            // Ejecutar cada 5 minutos
            Timer.Period = 5 * 60 * 1000; // 5 minutos en milisegundos
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var logger = workerContext.ServiceProvider
                .GetRequiredService<ILogger<EnviarNotificacionesEmailWorker>>();

            logger.LogInformation("Iniciando envío de notificaciones por email...");

            try
            {
                await EnviarNotificacionesPendientesAsync(workerContext.ServiceProvider);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al enviar notificaciones por email");
            }
        }

        private async Task EnviarNotificacionesPendientesAsync(IServiceProvider serviceProvider)
        {
            var unitOfWorkManager = serviceProvider.GetRequiredService<IUnitOfWorkManager>();
            var notificacionRepository = serviceProvider.GetRequiredService<IRepository<Notificacion, Guid>>();
            var usuarioRepository = serviceProvider.GetRequiredService<IRepository<Usuario, Guid>>();
            var emailService = serviceProvider.GetRequiredService<EmailService>();
            var logger = serviceProvider.GetRequiredService<ILogger<EnviarNotificacionesEmailWorker>>();

            using (var uow = unitOfWorkManager.Begin(requiresNew: true))
            {
                // Obtener todas las notificaciones no enviadas por mail
                var queryable = await notificacionRepository.GetQueryableAsync();
                var notificacionesPendientes = queryable
                    .Where(n => !n.EnviadaPorMail)
                    .OrderBy(n => n.CreationTime)
                    .Take(100) // Procesar máximo 100 por ejecución
                    .ToList();

                if (!notificacionesPendientes.Any())
                {
                    logger.LogInformation("No hay notificaciones pendientes de envío");
                    await uow.CompleteAsync();
                    return;
                }

                logger.LogInformation("Procesando {Count} notificaciones pendientes", notificacionesPendientes.Count);

                // Agrupar por usuario para verificar preferencias
                var notificacionesPorUsuario = notificacionesPendientes.GroupBy(n => n.UserId);

                foreach (var grupo in notificacionesPorUsuario)
                {
                    var userId = grupo.Key;
                    var notificaciones = grupo.ToList();

                    try
                    {
                        // Obtener usuario y sus preferencias
                        var usuario = await usuarioRepository.GetAsync(userId);

                        // Solo enviar si el usuario tiene habilitado el email
                        if (!usuario.Preferencias.RecibirPorEmail)
                        {
                            // Marcar como "enviadas" aunque no se enviaron (usuario no quiere emails)
                            foreach (var notif in notificaciones)
                            {
                                notif.MarcarComoEnviadaPorMail();
                            }
                            continue;
                        }

                        // Verificar frecuencia
                        if (usuario.Preferencias.Frecuencia == FrecuenciaNotificacion.Semanal)
                        {
                            // Si es semanal, saltear (se procesa en otro worker)
                            continue;
                        }

                        // Enviar cada notificación inmediatamente
                        foreach (var notif in notificaciones)
                        {
                            try
                            {
                                await emailService.EnviarNotificacionDestinoAsync(
                                    usuario.Email,
                                    notif.NombreDestino,
                                    notif.Titulo,
                                    notif.Mensaje
                                );

                                notif.MarcarComoEnviadaPorMail();
                                logger.LogInformation(
                                    "Email enviado a {Email} - Notificación: {Titulo}",
                                    usuario.Email,
                                    notif.Titulo);
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex,
                                    "Error al enviar email a {Email} - Notificación: {NotifId}",
                                    usuario.Email,
                                    notif.Id);
                                // No marcar como enviada si falló
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error al procesar notificaciones para usuario {UserId}", userId);
                    }
                }

                await uow.CompleteAsync();
                logger.LogInformation("Procesamiento de notificaciones completado");
            }
        }
    }
}
