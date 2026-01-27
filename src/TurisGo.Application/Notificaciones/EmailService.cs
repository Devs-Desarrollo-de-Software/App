using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.TextTemplating;

namespace TurisGo.Notificaciones
{
    // Servicio para envío de notificaciones por correo electrónico
    // Gestiona el envío de emails de notificaciones y resúmenes semanales
    public class EmailService : ITransientDependency
    {
        // Servicio de ABP para envío de correos electrónicos
        private readonly IEmailSender _emailSender;

        // Renderizador de plantillas para generar HTML de los emails
        private readonly ITemplateRenderer _templateRenderer;

        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IEmailSender emailSender,
            ITemplateRenderer templateRenderer,
            ILogger<EmailService> logger)
        {
            _emailSender = emailSender;
            _templateRenderer = templateRenderer;
            _logger = logger;
        }

        // Envía un email de notificación sobre cambios en un destino
        public async Task EnviarNotificacionDestinoAsync(
            string destinatario,
            string nombreDestino,
            string tituloNotificacion,
            string mensajeNotificacion)
        {
            try
            {
                var subject = $"TurisGo - {tituloNotificacion}";

                var body = await GenerarHtmlNotificacionAsync(
                    nombreDestino,
                    tituloNotificacion,
                    mensajeNotificacion);

                await _emailSender.SendAsync(
                    to: destinatario,
                    subject: subject,
                    body: body,
                    isBodyHtml: true
                );

                _logger.LogInformation(
                    "Email de notificación enviado a {Destinatario} sobre {Destino}",
                    destinatario,
                    nombreDestino);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al enviar email de notificación a {Destinatario}",
                    destinatario);
                throw;
            }
        }

        // Envía un resumen semanal de notificaciones agrupadas por destino
        public async Task EnviarResumenSemanalAsync(
            string destinatario,
            List<NotificacionResumenDto> notificaciones)
        {
            try
            {
                var subject = "TurisGo - Resumen Semanal de Notificaciones";

                var body = await GenerarHtmlResumenSemanalAsync(notificaciones);

                await _emailSender.SendAsync(
                    to: destinatario,
                    subject: subject,
                    body: body,
                    isBodyHtml: true
                );

                _logger.LogInformation(
                    "Resumen semanal enviado a {Destinatario} con {Cantidad} notificaciones",
                    destinatario,
                    notificaciones.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al enviar resumen semanal a {Destinatario}",
                    destinatario);
                throw;
            }
        }

        private async Task<string> GenerarHtmlNotificacionAsync(
            string nombreDestino,
            string titulo,
            string mensaje)
        {
            // Template HTML simple para notificación
            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <title>TurisGo - Notificación</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 20px;
        }}
        .container {{
            background-color: #ffffff;
            max-width: 600px;
            margin: 0 auto;
            padding: 30px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }}
        .header {{
            background-color: #007bff;
            color: #ffffff;
            padding: 20px;
            text-align: center;
            border-radius: 8px 8px 0 0;
            margin: -30px -30px 20px -30px;
        }}
        .content {{
            padding: 20px 0;
        }}
        .destino {{
            font-size: 18px;
            font-weight: bold;
            color: #007bff;
            margin-bottom: 10px;
        }}
        .mensaje {{
            font-size: 14px;
            line-height: 1.6;
            color: #333333;
        }}
        .footer {{
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid #eeeeee;
            font-size: 12px;
            color: #999999;
            text-align: center;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>TurisGo</h1>
            <p>{titulo}</p>
        </div>
        <div class=""content"">
            <div class=""destino"">📍 {nombreDestino}</div>
            <div class=""mensaje"">
                {mensaje}
            </div>
        </div>
        <div class=""footer"">
            <p>Este es un correo automático de TurisGo.</p>
            <p>Si deseas dejar de recibir notificaciones, actualiza tus preferencias en la aplicación.</p>
        </div>
    </div>
</body>
</html>";

            return await Task.FromResult(html);
        }

        private async Task<string> GenerarHtmlResumenSemanalAsync(List<NotificacionResumenDto> notificaciones)
        {
            var notificacionesHtml = string.Join("", notificaciones.ConvertAll(n =>
                $@"
                <div style=""padding: 15px; border-bottom: 1px solid #eeeeee;"">
                    <div style=""font-weight: bold; color: #007bff;"">📍 {n.NombreDestino}</div>
                    <div style=""font-size: 14px; margin-top: 5px;"">{n.Titulo}</div>
                    <div style=""font-size: 12px; color: #666666; margin-top: 5px;"">{n.Mensaje}</div>
                    <div style=""font-size: 11px; color: #999999; margin-top: 5px;"">{n.Fecha:dd/MM/yyyy HH:mm}</div>
                </div>"));

            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <title>TurisGo - Resumen Semanal</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 20px;
        }}
        .container {{
            background-color: #ffffff;
            max-width: 600px;
            margin: 0 auto;
            padding: 30px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }}
        .header {{
            background-color: #007bff;
            color: #ffffff;
            padding: 20px;
            text-align: center;
            border-radius: 8px 8px 0 0;
            margin: -30px -30px 20px -30px;
        }}
        .footer {{
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid #eeeeee;
            font-size: 12px;
            color: #999999;
            text-align: center;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>TurisGo</h1>
            <p>Resumen Semanal de Notificaciones</p>
        </div>
        <div style=""padding: 20px 0;"">
            <p style=""font-size: 14px; color: #666666;"">
                Tienes {notificaciones.Count} notificaciones nuevas en tus destinos favoritos:
            </p>
            {notificacionesHtml}
        </div>
        <div class=""footer"">
            <p>Este es un correo automático de TurisGo.</p>
            <p>Si deseas cambiar la frecuencia de notificaciones, actualiza tus preferencias en la aplicación.</p>
        </div>
    </div>
</body>
</html>";

            return await Task.FromResult(html);
        }
    }

    /// <summary>
    /// DTO para resumen de notificación
    /// </summary>
    public class NotificacionResumenDto
    {
        public string NombreDestino { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
    }
}
