using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurisGo.Destinos;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace TurisGo.Favoritos
{
    [Authorize]
    public class FavoritoAppService : ApplicationService, IFavoritoAppService
    {
        private readonly IRepository<Favorito, Guid> _favoritoRepository;
        private readonly IRepository<Destino, Guid> _destinoRepository;

        public FavoritoAppService(
            IRepository<Favorito, Guid> favoritoRepository,
            IRepository<Destino, Guid> destinoRepository)
        {
            _favoritoRepository = favoritoRepository;
            _destinoRepository = destinoRepository;
        }

        /// <summary>
        /// 6.1. Agregar destino a lista de favoritos
        /// </summary>
        public async Task<FavoritoDto> AgregarFavoritoAsync(CrearFavoritoDto input)
        {
            if (!CurrentUser.IsAuthenticated)
                throw new UnauthorizedAccessException("Debe estar autenticado para agregar favoritos.");

            var userId = CurrentUser.Id!.Value;

            // Verificar que el destino existe
            var destino = await _destinoRepository.FindAsync(input.DestinoId);
            if (destino == null)
                throw new UserFriendlyException("El destino especificado no existe.");

            // Verificar que no exista ya el favorito
            var yaExiste = await _favoritoRepository.FirstOrDefaultAsync(f =>
                f.DestinoId == input.DestinoId &&
                f.UserId == userId);

            if (yaExiste != null)
                throw new UserFriendlyException("Este destino ya está en tu lista de favoritos.");

            // Crear el favorito
            var favorito = new Favorito(
                GuidGenerator.Create(),
                userId,
                input.DestinoId
            );

            // Guardar
            var nuevoFavorito = await _favoritoRepository.InsertAsync(favorito, autoSave: true);

            // Retornar
            return ObjectMapper.Map<Favorito, FavoritoDto>(nuevoFavorito);
        }

        // 6.2. Eliminar destino de lista de favoritos
        public async Task DeleteAsync(Guid favoritoId)
        {
            // Validar si es usuario esta autenticado
            if (!CurrentUser.IsAuthenticated)
                throw new UnauthorizedAccessException("Debe estar autenticado para eliminar favoritos.");

            var userId = CurrentUser.Id!.Value;

            // Validar si existe el favorito y pertenece al usuario
            var favorito = await _favoritoRepository.FindAsync(favoritoId);
            if (favorito == null || favorito.UserId != userId)
                throw new UserFriendlyException("El favorito especificado no existe o no pertenece al usuario.");

            // Eliminar favorito
            await _favoritoRepository.DeleteAsync(favorito, autoSave: true);
        }

        // 6.3. Listar favoritos por usuario
        public async Task<ListaFavoritosDto> GetListAsync()
        {
            // Validar si es usuario esta autenticado
            if (!CurrentUser.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Debe estar autenticado para ver favoritos.");
            }

            var userId = CurrentUser.Id!.Value;

            // Obtener la lista de favoritos del usuario
            var queryable = await _favoritoRepository.GetQueryableAsync();

           var favoritos = await AsyncExecuter.ToListAsync(
                queryable.Where(f => f.UserId == userId)
                .Include(f => f.Destino)
                .OrderByDescending(f => f.CreationTime)
            );

            // Mapear a DTOs
            var favoritosDto = ObjectMapper.Map<List<Favorito>, List<FavoritoConDestinoDto>>(favoritos);

            // Retorna lista vacia si no hay favoritos
            if (favoritosDto == null)
            {
                return new ListaFavoritosDto
                {
                    UserId = userId,
                    Favoritos = new List<FavoritoConDestinoDto>()
                };
            }

            // Retornar
            return new ListaFavoritosDto
            {
                UserId = userId,
                Favoritos = favoritosDto
            };

        }
    }
}