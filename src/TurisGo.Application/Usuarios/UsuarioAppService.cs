using Abp.Runtime.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace TurisGo.Usuarios
{
    [Authorize]
    public class UsuarioAppService : ApplicationService, IUsuarioAppService 
    {
        private readonly Volo.Abp.Domain.Repositories.IRepository<Usuario, Guid> _usuarioRepository;
        private readonly IdentityUserManager _userManager;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly ILookupNormalizer _lookupNormalizer;

        public UsuarioAppService(
            Volo.Abp.Domain.Repositories.IRepository<Usuario, Guid> usuarioRepository,
            IdentityUserManager userManager,
            IIdentityUserRepository identityUserRepository,
            ILookupNormalizer lookupNormalizer)
        {
            _usuarioRepository = usuarioRepository;
            _userManager = userManager;
            _identityUserRepository = identityUserRepository;
            _lookupNormalizer = lookupNormalizer;
        }

        [Authorize(Roles = "admin")]
        public async Task<UsuarioDto> RegistrarUsuarioAsync(CrearUsuarioDto input)
        {

            // Validar que el nombre de usuario no exista
            var existeUsuario = await _usuarioRepository.
                AnyAsync(u => u.NombreUsuario == input.NombreUsuario);

            if (existeUsuario)
            {
                throw new BusinessException($"El nombre de usuario '{ input.NombreUsuario}' ya esta en uso"); // 403
            }

            // Validar que el email no exista en IdentityUser
            var existeEmail = await _usuarioRepository.
                AnyAsync(u => u.Email == input.Email);

             if (existeEmail)
             {   
                throw new BusinessException(
                    $"El correo electrónico '{ input.Email}' ya esta en uso");  // 403
             }

            // Validar que el nombre de usuario no exista en IdentityUser
            var normalizedUserName = _lookupNormalizer.NormalizeName(input.NombreUsuario);
            var existeIdentityUser = await _identityUserRepository.
                FindByNormalizedUserNameAsync(normalizedUserName);

            if (existeIdentityUser != null)
            {
               throw new UserFriendlyException(
                    $"El nombre de usuario '{ input.NombreUsuario}' ya esta en uso");
            }

            // Crear usuario (AbpUsers)
            var identityUser = new IdentityUser(
                GuidGenerator.Create(),
                input.NombreUsuario,
                input.Email,
                CurrentTenant.Id
            );

            // Asignar la contraseña
            var identityResult = await _userManager.CreateAsync(
                identityUser,
                input.Password
            );

            if (!identityResult.Succeeded)
            {
                var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                throw new UserFriendlyException(
                    $"Error al crear el usuario: {errors}"
                );
            }

            // Asignar rol
            var roleName =input.Rol == TipoRol.Administrador ? "Admin" : "User";
            await _userManager.AddToRoleAsync(identityUser, roleName);


            // Crear el usuario en la tabla Usuario (AppUsuarios)
            var usuario = new Usuario(
                GuidGenerator.Create(),
                input.NombreCompleto,
                input.NombreUsuario,
                identityUser.Id,    // Conexion con AbpUsers
                input.Email,
                input.Rol,
                input.FotoPerfilUrl
            );

            // Guardar en la base de datos
            await _usuarioRepository.InsertAsync(usuario, autoSave: true);

            // Mapear a DTO y retornar DTO
            return ObjectMapper.Map<Usuario, UsuarioDto>(usuario);

        }

    }
}
