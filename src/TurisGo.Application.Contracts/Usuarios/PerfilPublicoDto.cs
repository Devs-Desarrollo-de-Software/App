using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace TurisGo.Usuarios
{
    // DTO para exponer el perfil público de un usuario (sin información sensible)
    public class PerfilPublicoDto
    {
        public string NombreCompleto { get; set; }
        public string NombreUsuario { get; set; }
        public string FotoPerfilUrl { get; set; }
    }
}
