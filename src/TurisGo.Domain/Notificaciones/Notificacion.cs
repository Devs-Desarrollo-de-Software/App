using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisGo.Usuarios;
using Volo.Abp.Domain.Entities.Auditing;

namespace TurisGo.Notificaciones
{
    public class Notificacion : FullAuditedAggregateRoot<Guid>, IUserOwned
    {
        public Guid UserId { get; set; }
        public Guid DestinoId { get; private set; }
        public string Titulo { get; private set; }
        public string Mensaje { get; private set; }
        public TipoNotificacion Tipo { get; private set; }
        public bool Leida { get; private set; }
        public DateTime? FechaLectura { get; private set; }
        public bool EnviadaPorMail { get; private set; }
        public DateTime? FechaEnvioMail { get; private set; }
        public string NombreDestino { get; private set; }


        protected Notificacion() { }  // Constructor protegido para EF Core

        public Notificacion(Guid id, Guid usuarioId, Guid destinoId, string titulo, string mensaje, TipoNotificacion tipo, string nombreDestino)
            : base(id)
        {
            UserId = usuarioId;
            DestinoId = destinoId;
            SetTitulo(titulo);
            SetMensaje(mensaje);
            Tipo = tipo;
            Leida = false;
            NombreDestino = nombreDestino;
        }

        // Marca la notificacion como leída y establece la fecha de lectura
        public void MarcarComoLeida()
        {
            if (!Leida) 
            {
                Leida = true;
                FechaLectura = DateTime.UtcNow;
            }
        }

        // Marca la notificacion como no leída y limpia la fecha de lectura
        public void MarcarComoNoLeida()
        {
            if (Leida) 
            {
                Leida = false;
                FechaLectura = null;
            }
        }

        // Registra que la notificacion fue enviada por mail y establece la fecha de envio
        public void MarcarComoEnviadaPorMail()
        {
            EnviadaPorMail = true;
            FechaEnvioMail = DateTime.UtcNow;
        }

        // Valida y establece el mensaje de la notificación
        private void SetTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                throw new ArgumentException("El título no puede estar vacío.", nameof(titulo));
            }

            if (titulo.Length > 200)
            {
                throw new ArgumentException("El título no puede exceder los 200 caracteres.", nameof(titulo));
            }

            Titulo = titulo;
        }

        // Valida y establece el mensaje de la notificación
        private void SetMensaje(string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje))
            {
                throw new ArgumentException("El mensaje no puede estar vacío.", nameof(mensaje));
            }
            if (mensaje.Length > 1000)
            {
                throw new ArgumentException("El mensaje no puede exceder los 1000 caracteres.", nameof(mensaje));
            }
            Mensaje = mensaje;
        }

        // Determina si la notificacion debe ser archivada
        public bool DebeSerArchivada()
        {
            return CreationTime.AddDays(30) < DateTime.UtcNow;
        }




    }
}
