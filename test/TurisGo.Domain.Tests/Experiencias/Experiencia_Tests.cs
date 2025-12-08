using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Validation;
using Xunit;

namespace TurisGo.Experiencias
{
    public class Experiencia_Tests
    {
        private readonly Guid _userId = Guid.NewGuid();
        private readonly Guid _destinoId = Guid.NewGuid();


        [Fact]
        public void Should_Create_Experiencia_With_Valid_Data()
        {
            // Arrange
            var id = Guid.NewGuid();
            var titulo = "Una experiencia maravillosa";
            var descripcion = "Disfruté mucho de mi visita a este destino.";

            // Act
            var experiencia = new Experiencia(
                id,
                _userId,
                _destinoId,
                titulo,
                descripcion,
                TipoValoracion.Positiva
            );

            // Assert
            experiencia.ShouldNotBeNull();
            experiencia.Id.ShouldBe(id);
            experiencia.UserId.ShouldBe(_userId);
            experiencia.DestinoId.ShouldBe(_destinoId);
            experiencia.Titulo.ShouldBe(titulo);
            experiencia.Descripcion.ShouldBe(descripcion);
            experiencia.Valoracion.ShouldBe(TipoValoracion.Positiva);

        }

        [Fact]
        public void Should_Throw_Exception_When_UserId_Is_Empty()
        {
            // Act & Assert
            Should.Throw<AbpValidationException>(() =>
            {
                var experiencia = new Experiencia(
                    Guid.NewGuid(),
                    Guid.Empty,
                    _destinoId,
                    "Titulo Valido",
                    "Descripcion Valida",
                    TipoValoracion.Positiva
                );
            });
        }

        [Fact]
        public void Should_Throw_Exception_When_DestinoId_Is_Empty()
        {
            // Act & Assert
            Should.Throw<AbpValidationException>(() =>
            {
                var experiencia = new Experiencia(
                    Guid.NewGuid(),
                    _userId,
                    Guid.Empty,
                    "Titulo Valido",
                    "Descripcion Valida",
                    TipoValoracion.Positiva
                );
            });
        }

        [Fact]
        public void Should_Throw_Exception_When_Titulo_Exceeds_MaxLength()
        {
            // Arrange
            var tituloLargo = new string('A', 101); // 101 caracteres

            // Act & Assert
            Should.Throw<AbpValidationException>(() =>
            {
                var experiencia = new Experiencia(
                    Guid.NewGuid(),
                    _userId,
                    _destinoId,
                    tituloLargo,
                    "Descripcion Valida",
                    TipoValoracion.Positiva
                );
            });
        }


        [Fact]
        public void Should_Update_Experiencia_With_Valid_Data()
        {
            // Arrange
            var experiencia = new Experiencia(
                Guid.NewGuid(),
                _userId,
                _destinoId,
                "Titulo Inicial",
                "Descripcion Inicial",
                TipoValoracion.Positiva
            );

            var updateDto = new UpdateExperienciaDto
            {
                Titulo = "Titulo Actualizado",
                Descripcion = "Descripcion Actualizada",
                Valoracion = TipoValoracion.Negativa
            };


            // Act
            experiencia.SetTitulo(updateDto.Titulo);
            experiencia.SetDescripcion(updateDto.Descripcion);
            experiencia.SetValoracion(updateDto.Valoracion);


            // Assert
            experiencia.Titulo.ShouldBe("Titulo Actualizado");
            experiencia.Descripcion.ShouldBe("Descripcion Actualizada");
            experiencia.Valoracion.ShouldBe(TipoValoracion.Negativa);
        }

        [Fact]
        public void Should_Throw_Exception_When_Titulo_Is_Null()
        {
            // Arrange
            var experiencia = new Experiencia(
                Guid.NewGuid(),
                _userId,
                _destinoId,
                "Titulo Inicial",
                "Descripcion Inicial",
                TipoValoracion.Positiva
            );

            // Act & Assert
            Should.Throw<AbpValidationException>(() =>
            {
                experiencia.SetTitulo(null);
            });

        }

        [Fact]
        public void Should_Throw_Exception_When_Descripcion_Exceeds_MaxLength()
        {
            // Arrange
            var experiencia = new Experiencia(
                Guid.NewGuid(),
                _userId,
                _destinoId,
                "Titulo Inicial",
                "Descripcion Inicial",
                TipoValoracion.Positiva
            );

            var descripcionLarga = new string('B', 501); // 501 caracteres

            // Act & Assert
            Should.Throw<AbpValidationException>(() =>
            {
                experiencia.SetDescripcion(descripcionLarga);
            });


        }
    }
}