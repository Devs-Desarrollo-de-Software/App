using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TurisGo.Favoritos
{
    public interface IFavoritoAppService : IApplicationService
    {
        // 6.1. Agregar destino a lista de favoritos
        Task<FavoritoDto> AgregarFavoritoAsync(CrearFavoritoDto input);

        // 6.2. Eliminar destino de lista de favoritos
        Task DeleteAsync(Guid favoritoId);

        // 6.3. Consultar lista personal de favoritos
        Task<ListaFavoritosDto> GetListAsync();
    }
}
