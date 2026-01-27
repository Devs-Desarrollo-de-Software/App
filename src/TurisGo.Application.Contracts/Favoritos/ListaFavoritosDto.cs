using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisGo.Destinos;

namespace TurisGo.Favoritos
{
    public class ListaFavoritosDto
    {
        public Guid UserId { get; set; }
        public List<FavoritoConDestinoDto> Favoritos { get; set; }
    }
}
