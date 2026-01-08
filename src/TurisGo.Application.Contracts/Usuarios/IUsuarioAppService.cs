using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TurisGo.Usuarios
{
    public interface IUsuarioAppService : IApplicationService
    {

        // 1.1 Registrar nuevo usuario
        Task<UsuarioDto> RegistrarUsuarioAsync(CrearUsuarioDto input);
    }
}
