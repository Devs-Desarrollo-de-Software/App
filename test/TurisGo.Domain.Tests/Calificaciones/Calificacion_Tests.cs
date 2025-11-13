using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

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
            calificacion.Puntuacion.ShouldBe(5);
            calificacion.Comentario.ShouldBe("Excelente destino");
        }

        [Fact]
        public void Should_Not_Accept_Invalid_Puntuacion()
        {
            // Act & Assert
            Should.Throw<ArgumentException>(() =>
            new Calificacion(
                Guid.NewGuid(),
                _destinoId,
                _userId,
                6,
                "test"
                )
            );
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
        public void Should_Not_Accept_Large_Comentario()
        {
            //Arrange
            var comentarioLargo = new string('A', 1001); // El limite es 1000

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
 
    }
}
