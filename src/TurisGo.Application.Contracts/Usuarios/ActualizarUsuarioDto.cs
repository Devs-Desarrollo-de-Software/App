using System;

namespace TurisGo.Usuarios
{
    // DTO para que el administrador actualice datos de un usuario
    public class ActualizarUsuarioDto
    {
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string FotoPerfilUrl { get; set; }
        public TipoRol Rol { get; set; }
        public bool EstaActivo { get; set; }
    }
}
