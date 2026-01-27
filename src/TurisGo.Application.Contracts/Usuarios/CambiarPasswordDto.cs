using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisGo.Usuarios
{
    public class CambiarPasswordDto
    {
        [Required(ErrorMessage = "La contraseña actual es requerida.")]
        public string PasswordActual { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es requerida.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La nueva contraseña debe tener entre 6 y 100 caracteres.")]
        public string NuevoPassword { get; set; }

        [Required(ErrorMessage = "La confirmación de la nueva contraseña es requerida.")]
        [Compare("NuevoPassword", ErrorMessage = "La nueva contraseña y su confirmación no coinciden.")]
        public string ConfirmarNuevoPassword { get; set; }
    }
}
