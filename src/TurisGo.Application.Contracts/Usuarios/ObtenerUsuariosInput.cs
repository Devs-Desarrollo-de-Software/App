using Volo.Abp.Application.Dtos;

namespace TurisGo.Usuarios
{
    // DTO para filtrar y paginar la lista de usuarios
    public class ObtenerUsuariosInput : PagedAndSortedResultRequestDto
    {
        public string? Filtro { get; set; }
        public TipoRol? Rol { get; set; }
        public bool? EstaActivo { get; set; }
    }
}
