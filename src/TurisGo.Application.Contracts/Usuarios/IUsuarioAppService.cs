using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace TurisGo.Usuarios
{
    // Contrato de servicios para la gestión de usuarios
    // Define todas las operaciones disponibles para usuarios y administradores
    public interface IUsuarioAppService : IApplicationService
    {
        // ==================== OPERACIONES DE USUARIO ====================

        // Registra un nuevo usuario en el sistema (solo Admin)
        Task<UsuarioDto> RegistrarUsuarioAsync(CrearUsuarioDto input);

        // Obtiene el perfil del usuario autenticado
        Task<UsuarioDto> ObtenerPerfilActualAsync();

        // Actualiza el perfil del usuario autenticado (nombre, email, foto)
        Task<UsuarioDto> ActualizarPerfilAsync(ActualizarPerfilDto input);

        // Actualiza las preferencias de notificación del usuario autenticado
        Task<UsuarioDto> ActualizarPreferenciasAsync(ActualizarPreferenciasDto input);

        // Cambia la contraseña del usuario autenticado
        Task CambiarPasswordAsync(CambiarPasswordDto input);

        // Obtiene el perfil público de otro usuario por nombre de usuario
        Task<PerfilPublicoDto> ObtenerPerfilPublicoAsync(string nombreUsuario);

        // ==================== GESTIÓN DE USUARIOS (ADMIN) ====================

        // Lista todos los usuarios con paginación y filtros (solo Admin)
        Task<PagedResultDto<UsuarioDto>> ObtenerTodosUsuariosAsync(ObtenerUsuariosInput input);

        // Obtiene un usuario específico por ID (solo Admin)
        Task<UsuarioDto> ObtenerUsuarioPorIdAsync(Guid id);

        // Actualiza los datos de un usuario (solo Admin)
        Task<UsuarioDto> ActualizarUsuarioAsync(Guid id, ActualizarUsuarioDto input);

        // Desactiva un usuario (solo Admin)
        Task EliminarUsuarioAsync(Guid id);

        // Reactiva un usuario inactivo (solo Admin)
        Task<UsuarioDto> ReactivarUsuarioAsync(CrearUsuarioDto input);
    }
}
