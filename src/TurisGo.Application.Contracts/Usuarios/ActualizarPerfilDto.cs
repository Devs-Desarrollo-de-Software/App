using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisGo.Usuarios
{
    public class ActualizarPerfilDto
    {
        [StringLength(100, ErrorMessage = "El nombre completo no puede exceder 100 caracteres")]
        public string NombreCompleto { get; set; }

        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        public string Email { get; set; }

        [StringLength(500, ErrorMessage = "La URL de la foto no puede exceder 500 caracteres")]
        [Url(ErrorMessage = "La URL de la foto no es válida")]
        public string? FotoPerfilUrl { get; set; }

    }
}
