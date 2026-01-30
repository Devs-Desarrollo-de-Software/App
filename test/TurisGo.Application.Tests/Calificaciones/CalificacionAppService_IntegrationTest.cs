using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TurisGo.Destinos;
using TurisGo.EntityFrameworkCore;
using TurisGo.Usuarios;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using Volo.Abp.Validation;
using Xunit;

namespace TurisGo.Calificaciones
{
    [Collection("IntegrationTest")]
    public abstract class CalificacionAppService_IntegrationTest<TStartupModule> : TurisGoApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {

        private readonly ICalificacionAppService _calificaciones;
        private readonly IDestinoAppService _destinos;
        private readonly IDbContextProvider<TurisGoDbContext> _DbContextProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICurrentPrincipalAccessor _principalAccessor;

        protected CalificacionAppService_IntegrationTest()
        {
            _calificaciones = GetRequiredService<ICalificacionAppService>();
            _destinos = GetRequiredService<IDestinoAppService>();
            _DbContextProvider = GetRequiredService<IDbContextProvider<TurisGoDbContext>>();
            _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
            _principalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
        }

        // ==== Helper para simular usuario autenticado ====
        private IDisposable UseUser(Guid userId, string userName = "testuser")
        {
            var identity = new ClaimsIdentity(
                new[]
                {
                    new Claim(AbpClaimTypes.UserId, userId.ToString()),
                    new Claim(AbpClaimTypes.UserName, userName)
                },
                 authenticationType: "TestAuth"
                );
            var principal = new ClaimsPrincipal(identity);
            return _principalAccessor.Change(principal);
        }

        private IDisposable UseAnonymous()
        {
            //Limpia el prinicpal para simular no autenticado
            return _principalAccessor.Change(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        private async Task<Guid> CrearDestinoAsync(string nombre = "Paris")
        {
            var input = new CreateUpdateDestinoDto
            {
                Nombre = nombre,
                Pais = "Francia",
                Poblacion = 2000000,
                Imagen = "https://ejemplo.com/paris.png",
                Coordenada = new CoordenadaDto { Latitud = 48.8566, Longitud = 2.3522 }
            };
            var dto = await _destinos.CreateAsync(input);
            return dto.Id;
        }


        // Requiere autenticacion --> si no hay usuario, debe fallar
        [Fact]
        public async Task Should_Require_Authentication_On_Create()
        {
            using (UseAnonymous())
            {
                var input = new CreateCalificacionDto
                {
                    DestinoId = Guid.NewGuid(),
                    Puntuacion = 5,
                    Comentario = "Excelente lugar!"
                };

                var ex = await Should.ThrowAsync<Exception>(async () =>
                {
                    await _calificaciones.CreateAsync(input);
                });

                (ex is AbpAuthorizationException || ex is UnauthorizedAccessException).ShouldBeTrue();
            }

        }

        [Fact]
        public async Task Should_Not_Allow_Duplicate_Rating_For_Same_Destino()
        {
            var user = Guid.NewGuid();
            var destinoId = await CrearDestinoAsync("Londres");

            using (UseUser(user, "user"))
            {
                // Primera calificación - OK
                await _calificaciones.CreateAsync(new CreateCalificacionDto
                {
                    DestinoId = destinoId,
                    Puntuacion = 5,
                    Comentario = "Primera"
                });

                // Segunda calificación mismo destino - DEBE FALLAR
                await Should.ThrowAsync<AbpValidationException>(async () =>
                    await _calificaciones.CreateAsync(new CreateCalificacionDto
                    {
                        DestinoId = destinoId,
                        Puntuacion = 4,
                        Comentario = "Intento duplicado"
                    })
                );
            }
        }

        [Fact]
        public async Task Should_Filter_Ratings_By_Current_User()
        {
            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();

            Guid d1_user1, d2_user1, d1_user2;

            using (UseUser(user1, "user1"))
            {
                d1_user1 = await CrearDestinoAsync("Tokio");
                d2_user1 = await CrearDestinoAsync("Paris");

                await _calificaciones.CreateAsync(new CreateCalificacionDto { 
                    DestinoId = d1_user1, 
                    Puntuacion = 5, 
                    Comentario = "A" });

                await _calificaciones.CreateAsync(new CreateCalificacionDto { 
                    DestinoId = d2_user1, 
                    Puntuacion = 4, 
                    Comentario = "B" });
            }

            using (UseUser(user2, "user2"))
            {
                d1_user2 = await CrearDestinoAsync("Roma");
                await _calificaciones.CreateAsync(new CreateCalificacionDto {
                    DestinoId = d1_user2, 
                    Puntuacion = 2, 
                    Comentario = "C" });
            }

            using (UseUser(user1, "user1"))
            {
                var listUser1 = await _calificaciones.GetListAsync(new PagedAndSortedResultRequestDto { MaxResultCount = 1000 });
                listUser1.ShouldNotBeNull();
                listUser1.Items.Count.ShouldBe(2);
                listUser1.Items.All(i => i.UserId == user1).ShouldBeTrue();
                listUser1.Items.Any(i => i.UserId == user2).ShouldBeFalse();
            }

            using (UseUser(user2, "user2"))
            {
                var listUser2 = await _calificaciones.GetListAsync(new PagedAndSortedResultRequestDto { MaxResultCount = 1000 });
                listUser2.ShouldNotBeNull();
                listUser2.Items.Count.ShouldBe(1);
                listUser2.Items.Single().UserId.ShouldBe(user2);
            }
        }

        [Fact]
        public async Task Owner_Can_Update_And_Delete_His_Own_Rating()
        {
            var user = Guid.NewGuid();
            var destinoId = await CrearDestinoAsync("Madrid");
            Guid califId;

            using (UseUser(user, "user"))
            {
                califId = (await _calificaciones.CreateAsync(new CreateCalificacionDto
                {
                    DestinoId = destinoId,
                    Puntuacion = 3,
                    Comentario = "Original"
                })).Id;

                // Usar UpdateCalificacionDto en lugar de CreateCalificacionDto
                var updated = await _calificaciones.UpdateAsync(califId, new UpdateCalificacionDto
                {
                    Puntuacion = 5,
                    Comentario = "Edit"
                });
                updated.Puntuacion.ShouldBe(5);
                updated.Comentario.ShouldBe("Edit");

                await _calificaciones.DeleteAsync(califId);

                var list = await _calificaciones.GetListAsync(new PagedAndSortedResultRequestDto { MaxResultCount = 100 });
                list.Items.Any(i => i.Id == califId).ShouldBeFalse();
            }
        }

        // Verificar que NO se puede actualizar calificación ajena
        [Fact]
        public async Task Should_Not_Allow_Update_Other_User_Rating()
        {
            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();
            var destinoId = await CrearDestinoAsync("Berlin");
            Guid califId;

            using (UseUser(user1, "user1"))
            {
                califId = (await _calificaciones.CreateAsync(new CreateCalificacionDto
                {
                    DestinoId = destinoId,
                    Puntuacion = 4,
                    Comentario = "De user1"
                })).Id;
            }

            using (UseUser(user2, "user2"))
            {
                await Should.ThrowAsync<EntityNotFoundException>(async () =>
                    await _calificaciones.UpdateAsync(califId, new UpdateCalificacionDto
                    {
                        Puntuacion = 1,
                        Comentario = "Intento de user2"
                    })
                );
            }
        }

        // Verificar que NO se puede eliminar calificación ajena
        [Fact]
        public async Task Should_Not_Allow_Delete_Other_User_Rating()
        {
            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();
            var destinoId = await CrearDestinoAsync("Ámsterdam");
            Guid califId;

            using (UseUser(user1, "user1"))
            {
                califId = (await _calificaciones.CreateAsync(new CreateCalificacionDto
                {
                    DestinoId = destinoId,
                    Puntuacion = 5,
                    Comentario = "De user1"
                })).Id;
            }

            using (UseUser(user2, "user2"))
            {
                await Should.ThrowAsync<EntityNotFoundException>(async () =>
                    await _calificaciones.DeleteAsync(califId)
                );
            }
        }


        [Fact]
        public async Task Should_Throw_When_Rating_Out_Of_Range()
        {
            var user = Guid.NewGuid();
            var destinoId = await CrearDestinoAsync("Bogotá");

            using (UseUser(user, "user"))
            {
                await Should.ThrowAsync<AbpValidationException>(() =>
                    _calificaciones.CreateAsync(new CreateCalificacionDto
                    {
                        DestinoId = destinoId,
                        Puntuacion = 999,
                        Comentario = "invalid"
                    }));
            }
        }

        // ← NUEVA: Verificar autenticación en Update
        [Fact]
        public async Task Should_Require_Authentication_On_Update()
        {
            var user = Guid.NewGuid();
            var destinoId = await CrearDestinoAsync("Milán");
            Guid califId;

            using (UseUser(user, "user"))
            {
                califId = (await _calificaciones.CreateAsync(new CreateCalificacionDto
                {
                    DestinoId = destinoId,
                    Puntuacion = 3,
                    Comentario = "Original"
                })).Id;
            }

            using (UseAnonymous())
            {
                var ex = await Should.ThrowAsync<Exception>(async () =>
                    await _calificaciones.UpdateAsync(califId, new UpdateCalificacionDto
                    {
                        Puntuacion = 5,
                        Comentario = "Intento"
                    })
                );

                (ex is AbpAuthorizationException || ex is UnauthorizedAccessException).ShouldBeTrue();
            }
        }

        // Verificar autenticación en Delete
        [Fact]
        public async Task Should_Require_Authentication_On_Delete()
        {
            var user = Guid.NewGuid();
            var destinoId = await CrearDestinoAsync("Florencia");
            Guid califId;

            using (UseUser(user, "user"))
            {
                califId = (await _calificaciones.CreateAsync(new CreateCalificacionDto
                {
                    DestinoId = destinoId,
                    Puntuacion = 4,
                    Comentario = "Original"
                })).Id;
            }

            using (UseAnonymous())
            {
                var ex = await Should.ThrowAsync<Exception>(async () =>
                    await _calificaciones.DeleteAsync(califId)
                );

                (ex is AbpAuthorizationException || ex is UnauthorizedAccessException).ShouldBeTrue();
            }
        }

        // ------------------ Operacion 5.4. Obtener promedio de calificacion de un destino ------------
        
        [Fact]
        public async Task Should_Calculate_Average_Rating_Correctly()
        {
            // Arrange
            var destinoId = await CrearDestinoAsync("Barcelona");
            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();
            var user3 = Guid.NewGuid();

            // Crear 3 calificaciones: 5, 4, 3 (promedio = 4.0)
            using (UseUser(user1, "user1"))
            {
                await _calificaciones.CreateAsync(new CreateCalificacionDto
                {
                    DestinoId = destinoId,
                    Puntuacion = 5,
                    Comentario = "Excelente"
                });
            }
            using (UseUser(user2, "user2"))
            {
                await _calificaciones.CreateAsync(new CreateCalificacionDto
                {
                    DestinoId = destinoId,
                    Puntuacion = 4,
                    Comentario = "Muy bueno"
                });
            }
            using (UseUser(user3, "user3"))
            {
                await _calificaciones.CreateAsync(new CreateCalificacionDto
                {
                    DestinoId = destinoId,
                    Puntuacion = 3,
                    Comentario = "Regular"
                });
            }

            // Act
            var promedio = await _calificaciones.GetPromedioAsync(destinoId);

            // Assert
            promedio.ShouldNotBeNull();
            promedio.DestinoId.ShouldBe(destinoId);
            promedio.TotalCalificaciones.ShouldBe(3);
            promedio.PromedioCalificacion.ShouldBe(4.0);
        }

        [Fact]
        public async Task Should_Return_Zero_When_No_Ratings()
        {
            // Arrange
            var destinoId = await CrearDestinoAsync("Madrid");

            // Act - No se crean calificaciones
            var promedio = await _calificaciones.GetPromedioAsync(destinoId);

            // Assert
            promedio.ShouldNotBeNull();
            promedio.DestinoId.ShouldBe(destinoId);
            promedio.TotalCalificaciones.ShouldBe(0);
            promedio.PromedioCalificacion.ShouldBe(0);
        }


        [Fact]
        public async Task Should_Throw_When_DestinoId_Is_Empty()
        {
            // Act & Assert
            await Should.ThrowAsync<AbpValidationException>(async () =>
                await _calificaciones.GetPromedioAsync(Guid.Empty)
            );
        }

        [Fact]
        public async Task Should_Allow_Anonymous_Access_To_Average()
        {
            // Arrange
            var destinoId = await CrearDestinoAsync("Londres");
            var user = Guid.NewGuid();

            using (UseUser(user, "user"))
            {
                await _calificaciones.CreateAsync(new CreateCalificacionDto
                {
                    DestinoId = destinoId,
                    Puntuacion = 5,
                    Comentario = "Test"
                });
            }

            // Act - Sin autenticación
            using (UseAnonymous())
            {
                var promedio = await _calificaciones.GetPromedioAsync(destinoId);

                // Assert
                promedio.ShouldNotBeNull();
                promedio.TotalCalificaciones.ShouldBe(1);
                promedio.PromedioCalificacion.ShouldBe(5.0);
            }
        }

        [Fact]
        public async Task Should_Not_Include_Ratings_From_Different_Destino()
        {
            // Arrange
            var destino1 = await CrearDestinoAsync("Viena");
            var destino2 = await CrearDestinoAsync("Praga");
            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();

            // Usuario 1 califica destino 1
            using (UseUser(user1, "user1"))
            {
                await _calificaciones.CreateAsync(new CreateCalificacionDto
                {
                    DestinoId = destino1,
                    Puntuacion = 5,
                    Comentario = "Destino 1"
                });
            }

            // Usuario 2 califica destino 2
            using (UseUser(user2, "user2"))
            {
                await _calificaciones.CreateAsync(new CreateCalificacionDto
                {
                    DestinoId = destino2,
                    Puntuacion = 1,
                    Comentario = "Destino 2"
                });
            }

            // Act - Obtener promedio solo del destino 1
            var promedio = await _calificaciones.GetPromedioAsync(destino1);

            // Assert - Solo debe contar la calificación del destino 1
            promedio.TotalCalificaciones.ShouldBe(1);
            promedio.PromedioCalificacion.ShouldBe(5.0); // Solo la de user1
        }

        [Fact]
        public async Task Should_Update_Average_When_New_Rating_Added()
        {
            // Arrange
            var destinoId = await CrearDestinoAsync("Ámsterdam");
            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();

            // Primera calificación
            using (UseUser(user1, "user1"))
            {
                await _calificaciones.CreateAsync(new CreateCalificacionDto
                {
                    DestinoId = destinoId,
                    Puntuacion = 5,
                    Comentario = "Primera"
                });
            }

            // Act 1 - Obtener promedio inicial
            var promedio1 = await _calificaciones.GetPromedioAsync(destinoId);
            promedio1.TotalCalificaciones.ShouldBe(1);
            promedio1.PromedioCalificacion.ShouldBe(5.0);

            // Agregar segunda calificación
            using (UseUser(user2, "user2"))
            {
                await _calificaciones.CreateAsync(new CreateCalificacionDto
                {
                    DestinoId = destinoId,
                    Puntuacion = 3,
                    Comentario = "Segunda"
                });
            }

            // Act 2 - Obtener promedio actualizado
            var promedio2 = await _calificaciones.GetPromedioAsync(destinoId);

            // Assert - El promedio debe actualizarse
            promedio2.TotalCalificaciones.ShouldBe(2);
            promedio2.PromedioCalificacion.ShouldBe(4.0); // (5+3)/2 = 4.0
        }

      
        [Fact]
        public async Task Should_Return_Empty_List_When_No_Comentarios()
        {
            // Arrange
            var destinoId = await CrearDestinoAsync("Madrid");

            // Act - No se crean comentarios
            var resultado = await _calificaciones.GetListComentariosAsync(destinoId);

            // Assert
            resultado.ShouldNotBeNull();
            resultado.DestinoId.ShouldBe(destinoId);
            resultado.Comentarios.ShouldNotBeNull();
            resultado.Comentarios.Count.ShouldBe(0);
        }
        

    }


}
        
