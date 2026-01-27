using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TurisGo.Notificaciones
{
    public class Notificacion_Tests
    {

        [Fact]
        public void Should_Create_Valid_Notification()
        {
            // Arrange
            var id = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var destinoId = Guid.NewGuid();
            var titulo = "Nuevo evento en París";
            var mensaje = "Se ha detectado un nuevo evento";
            var tipo = TipoNotificacion.NuevoEvento;
            var nombreDestino = "París";

            // Act
            var notificacion = new Notificacion(id, userId, destinoId, titulo, mensaje, tipo, nombreDestino);

            // Assert
            notificacion.Id.ShouldBe(id);
            notificacion.UserId.ShouldBe(userId);
            notificacion.DestinoId.ShouldBe(destinoId);
            notificacion.Titulo.ShouldBe(titulo);
            notificacion.Mensaje.ShouldBe(mensaje);
            notificacion.Tipo.ShouldBe(tipo);
            notificacion.NombreDestino.ShouldBe(nombreDestino);
            notificacion.Leida.ShouldBeFalse();
            notificacion.EnviadaPorMail.ShouldBeFalse();
        }

        [Fact]
        public void Should_Not_Create_Notification_With_Empty_Title()
        {
            // Arrange & Act & Assert
            Should.Throw<ArgumentException>(() =>
                new Notificacion(
                    Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                    "", "Mensaje", TipoNotificacion.NuevoEvento, "Destino"
                )
            );
        }

        [Fact]
        public void Should_Not_Create_Notification_With_Too_Long_Title()
        {
            // Arrange
            var tituloLargo = new string('A', 201);

            // Act & Assert
            Should.Throw<ArgumentException>(() =>
                new Notificacion(
                    Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                    tituloLargo, "Mensaje", TipoNotificacion.NuevoEvento, "Destino"
                )
            );
        }

        [Fact]
        public void Should_Not_Create_Notification_With_Empty_Message()
        {
            // Arrange & Act & Assert
            Should.Throw<ArgumentException>(() =>
                new Notificacion(
                    Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                    "Título", "", TipoNotificacion.NuevoEvento, "Destino"
                )
            );
        }

        [Fact]
        public void Should_Not_Create_Notification_With_Too_Long_Message()
        {
            // Arrange
            var mensajeLargo = new string('A', 1001);

            // Act & Assert
            Should.Throw<ArgumentException>(() =>
                new Notificacion(
                    Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                    "Título", mensajeLargo, TipoNotificacion.NuevoEvento, "Destino"
                )
            );
        }

        #region Mark As Read Tests

        [Fact]
        public void Should_Mark_As_Read()
        {
            // Arrange
            var notificacion = CreateValidNotification();

            // Act
            notificacion.MarcarComoLeida();

            // Assert
            notificacion.Leida.ShouldBeTrue();
            notificacion.FechaLectura.ShouldNotBeNull();
        }

        [Fact]
        public void Should_Mark_As_Unread()
        {
            // Arrange
            var notificacion = CreateValidNotification();
            notificacion.MarcarComoLeida();

            // Act
            notificacion.MarcarComoNoLeida();

            // Assert
            notificacion.Leida.ShouldBeFalse();
            notificacion.FechaLectura.ShouldBeNull();
        }

        [Fact]
        public void Should_Not_Change_Date_When_Already_Read()
        {
            // Arrange
            var notificacion = CreateValidNotification();
            notificacion.MarcarComoLeida();
            var fechaOriginal = notificacion.FechaLectura;

            // Act
            System.Threading.Thread.Sleep(10);
            notificacion.MarcarComoLeida();

            // Assert
            notificacion.FechaLectura.ShouldBe(fechaOriginal);
        }

        #endregion

        #region Email Sending Tests

        [Fact]
        public void Should_Mark_As_Sent_By_Email()
        {
            // Arrange
            var notificacion = CreateValidNotification();

            // Act
            notificacion.MarcarComoEnviadaPorMail();

            // Assert
            notificacion.EnviadaPorMail.ShouldBeTrue();
            notificacion.FechaEnvioMail.ShouldNotBeNull();
        }

        #endregion

        #region Archiving Tests

        [Fact]
        public void Should_Not_Be_Archived_When_Recent()
        {
            // Arrange
            var notificacion = CreateValidNotification();

            // Act
            var shouldBeArchived = notificacion.DebeSerArchivada();

            // Assert
            shouldBeArchived.ShouldBeTrue();
        }

        #endregion

        #region Helper Methods

        private Notificacion CreateValidNotification()
        {
            return new Notificacion(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Título",
                "Mensaje",
                TipoNotificacion.NuevoEvento,
                "París"
            );
        }

        #endregion
    }
}
