using System;
using System.Threading.Tasks;
using TurisGo.Usuarios;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace TurisGo
{
    public class UsuariosDataSeeder : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Usuario, Guid> _usuarioRepository;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IdentityUserManager _userManager;

        public UsuariosDataSeeder(
            IRepository<Usuario, Guid> usuarioRepository,
            IIdentityUserRepository identityUserRepository,
            IdentityUserManager userManager)
        {
            _usuarioRepository = usuarioRepository;
            _identityUserRepository = identityUserRepository;
            _userManager = userManager;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            // Buscar el usuario admin en IdentityUser
            var adminIdentityUser = await _identityUserRepository.FindByNormalizedUserNameAsync("ADMIN");

            if (adminIdentityUser != null)
            {
                // Verificar si ya existe en AppUsuarios
                var existeEnAppUsuarios = await _usuarioRepository
                    .AnyAsync(u => u.IdentityUserId == adminIdentityUser.Id);

                if (!existeEnAppUsuarios)
                {
                    // Crear el registro en AppUsuarios
                    var adminUsuario = new Usuario(
                        Guid.NewGuid(),
                        "Administrador del Sistema",  // NombreCompleto
                        adminIdentityUser.UserName,    // NombreUsuario
                        adminIdentityUser.Id,          // IdentityUserId
                        adminIdentityUser.Email,       // Email
                        TipoRol.Administrador,         // Rol
                        null                           // FotoPerfilUrl
                    );

                    await _usuarioRepository.InsertAsync(adminUsuario, autoSave: true);
                }
            }
        }
    }
}
