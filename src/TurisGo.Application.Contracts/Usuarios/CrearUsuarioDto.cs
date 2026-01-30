using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisGo.Usuarios
{
    // DTO para la creación de nuevos usuarios en el sistema
    // Solo puede ser utilizado por administradores
    public class CrearUsuarioDto
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        public string Email { get; set; }

        // La contraseña se requiere al crear el usuario
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 6,
                ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El rol es requerido")]
        public TipoRol Rol { get; set; }

        // URL de la foto de perfil (opcional)
        public string? FotoPerfilUrl { get; set; }
    }
}
