using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace TurisGo.Usuarios
{
    public class UsuarioDto : EntityDto<Guid>
    { 
        public string NombreCompleto { get; set; }
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public string FotoPerfilUrl { get; set; }
        public PreferenciasNotificacionDto Preferencias { get; set; }
        public TipoRol Rol { get; set; }
        public bool EstaActivo { get; set; }

    }
}
