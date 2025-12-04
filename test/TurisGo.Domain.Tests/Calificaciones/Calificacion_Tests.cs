using Shouldly;
using System;
using Xunit;
using TurisGo.Calificaciones;

namespace TurisGo.Calificaciones
{
    public class Calificacion_Tests
    {
        private readonly Guid _userId = Guid.NewGuid();
        private readonly Guid _destinoId = Guid.NewGuid();

        [Fact]
        public void Should_Create_Calificacion_With_Valid_Data()
        {
            //Arrange
            var id = Guid.NewGuid();
            //Act
            var calificacion = new Calificacion(
                id,
                _destinoId,
                _userId,
                5,
                "Excelente destino"
            );
            //Assert
            calificacion.Id.ShouldBe(id);
            calificacion.DestinoId.ShouldBe(_destinoId);
            calificacion.UserId.ShouldBe(_userId); // ← Agregar esta validación
            calificacion.Puntuacion.ShouldBe(5);
            calificacion.Comentario.ShouldBe("Excelente destino");
        }

        [Theory] // ← Cambiar a Theory para probar múltiples valores
        [InlineData(0)]
        [InlineData(6)]
        [InlineData(-1)]
        [InlineData(10)]
        public void Should_Not_Accept_Invalid_Puntuacion(int puntuacionInvalida)
        {
            // Act & Assert
            Should.Throw<ArgumentOutOfRangeException>(() => // ← Usar la excepción específica
                new Calificacion(
                    Guid.NewGuid(),
                    _destinoId,
                    _userId,
                    puntuacionInvalida,
                    "test"
                )
            );
        }

        [Theory] // ← Probar todos los valores válidos
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void Should_Accept_Valid_Puntuacion(int puntuacionValida)
        {
            // Act
            var calificacion = new Calificacion(
                Guid.NewGuid(),
                _destinoId,
                _userId,
                puntuacionValida,
                "test"
            );

            // Assert
            calificacion.Puntuacion.ShouldBe(puntuacionValida);
        }

        [Fact]
        public void Should_Accept_Comentario_Empty()
        {
            //Arrange
            var id = Guid.NewGuid();
            //Act
            var calificacion = new Calificacion(
                id,
                _destinoId,
                _userId,
                4,
                null
            );
            //Assert
            calificacion.Comentario.ShouldBeNull();
        }

        [Fact]
        public void Should_Accept_Comentario_With_Exactly_1000_Characters()
        {
            //Arrange
            var comentario1000 = new string('A', 1000); // Exactamente 1000

            //Act
            var calificacion = new Calificacion(
                Guid.NewGuid(),
                _destinoId,
                _userId,
                5,
                comentario1000
            );

            //Assert
            calificacion.Comentario.ShouldBe(comentario1000);
            calificacion.Comentario.Length.ShouldBe(1000);
        }

        [Fact]
        public void Should_Not_Accept_Large_Comentario()
        {
            //Arrange
            var comentarioLargo = new string('A', 1001); // El límite es 1000

            //Act & Assert
            Should.Throw<ArgumentException>(() =>
                new Calificacion(
                    Guid.NewGuid(),
                    _destinoId,
                    _userId,
                    5,
                    comentarioLargo)
            );
        }

        [Fact]
        public void Should_Trim_Comentario_Whitespace()
        {
            //Arrange & Act
            var calificacion = new Calificacion(
                Guid.NewGuid(),
                _destinoId,
                _userId,
                4,
                "  Comentario con espacios  "
            );

            //Assert
            calificacion.Comentario.ShouldBe("Comentario con espacios");
        }

        [Fact]
        public void Should_Not_Accept_DestinoId_Empty()
        {
            //Arrange
            var destinoVacio = Guid.Empty;

            //Act & Assert
            Should.Throw<ArgumentException>(() =>
                new Calificacion(
                    Guid.NewGuid(),
                    destinoVacio,
                    _userId,
                    5,
                    "test")
            );
        }

        [Fact]
        public void Should_Not_Accept_UserId_Empty()
        {
            //Arrange
            var userVacio = Guid.Empty;

            //Act & Assert
            Should.Throw<ArgumentException>(() =>
                new Calificacion(
                    Guid.NewGuid(),
                    _destinoId,
                    userVacio,
                    5,
                    "test")
            );
        }

        // ===== PRUEBAS PARA LOS MÉTODOS SetPuntuacion y SetComentario =====

        [Fact]
        public void Should_Update_Puntuacion_With_Valid_Value()
        {
            //Arrange
            var calificacion = new Calificacion(
                Guid.NewGuid(),
                _destinoId,
                _userId,
                3,
                "Comentario inicial"
            );

            //Act
            calificacion.SetPuntuacion(5);

            //Assert
            calificacion.Puntuacion.ShouldBe(5);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        [InlineData(-5)]
        public void Should_Not_Update_Puntuacion_With_Invalid_Value(int puntuacionInvalida)
        {
            //Arrange
            var calificacion = new Calificacion(
                Guid.NewGuid(),
                _destinoId,
                _userId,
                3,
                "Comentario inicial"
            );

            //Act & Assert
            Should.Throw<ArgumentOutOfRangeException>(() =>
                calificacion.SetPuntuacion(puntuacionInvalida)
            );
        }

        [Fact]
        public void Should_Update_Comentario_With_Valid_Value()
        {
            //Arrange
            var calificacion = new Calificacion(
                Guid.NewGuid(),
                _destinoId,
                _userId,
                4,
                "Comentario inicial"
            );

            //Act
            var nuevoComentario = "Comentario actualizado";
            calificacion.SetComentario(nuevoComentario);

            //Assert
            calificacion.Comentario.ShouldBe(nuevoComentario);
        }

        [Fact]
        public void Should_Allow_Null_When_Updating_Comentario()
        {
            //Arrange
            var calificacion = new Calificacion(
                Guid.NewGuid(),
                _destinoId,
                _userId,
                4,
                "Comentario inicial"
            );

            //Act
            calificacion.SetComentario(null);

            //Assert
            calificacion.Comentario.ShouldBeNull();
        }

        [Fact]
        public void Should_Not_Update_Comentario_When_Too_Long()
        {
            //Arrange
            var calificacion = new Calificacion(
                Guid.NewGuid(),
                _destinoId,
                _userId,
                4,
                "Comentario inicial"
            );
            var comentarioLargo = new string('A', 1001);

            //Act & Assert
            Should.Throw<ArgumentException>(() =>
                calificacion.SetComentario(comentarioLargo)
            );
        }

        [Fact]
        public void Should_Trim_Whitespace_When_Updating_Comentario()
        {
            //Arrange
            var calificacion = new Calificacion(
                Guid.NewGuid(),
                _destinoId,
                _userId,
                4,
                "Comentario inicial"
            );

            //Act
            calificacion.SetComentario("  Nuevo comentario  ");

            //Assert
            calificacion.Comentario.ShouldBe("Nuevo comentario");
        }

        // --- Operacion 5.4. Obtener promedio calificacion de un destino ---

        [Fact]
        public void PromedioCalificacionDto_Should_Accept_Zero_Values()
        {
            // Arrange & Act
            var dto = new PromedioCalificacionDto
            {
                DestinoId = Guid.NewGuid(),
                PromedioCalificacion = 0,
                TotalCalificaciones = 0
            };

            // Assert
            dto.PromedioCalificacion.ShouldBe(0);
            dto.TotalCalificaciones.ShouldBe(0);
        }


    }
}
