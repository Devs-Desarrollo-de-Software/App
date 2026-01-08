using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace TurisGo
{
    public class RolesDataSeeder : IDataSeedContributor, ITransientDependency
    {
        private readonly IIdentityRoleRepository _roleRepository;
        private readonly IdentityRoleManager _roleManager;

        public RolesDataSeeder(
            IIdentityRoleRepository roleRepository,
            IdentityRoleManager roleManager)
        {
            _roleRepository = roleRepository;
            _roleManager = roleManager;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            // Crear rol 'admin' si no existe
            if (await _roleRepository.FindByNormalizedNameAsync("ADMIN") == null)
            {
                var adminRole = new IdentityRole(
                    Guid.NewGuid(),
                    "admin",
                    context.TenantId
                )
                    {
                    IsDefault = false,
                    IsStatic = true,
                    IsPublic = false
                };

                await _roleRepository.InsertAsync(adminRole, autoSave: true);

            }

            // Crear rol 'user' si no existe
            if (await _roleRepository.FindByNormalizedNameAsync("USER") == null)
            {
                var userRole = new IdentityRole(
                    Guid.NewGuid(),
                    "user",
                    context.TenantId
                )
                {
                    IsDefault = true,   // Rol por defecto para nuevos usuarios
                    IsStatic = true,
                    IsPublic = false
                };

                await _roleRepository.InsertAsync(userRole, autoSave: true);
            }
        }
    }
}
