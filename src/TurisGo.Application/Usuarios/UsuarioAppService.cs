using Abp.Runtime.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisGo.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EntityFrameworkCore;
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
        private readonly IDbContextProvider<TurisGoDbContext> _dbContextProvider;

        public UsuarioAppService(
            Volo.Abp.Domain.Repositories.IRepository<Usuario, Guid> usuarioRepository,
            IdentityUserManager userManager,
            IIdentityUserRepository identityUserRepository,
            ILookupNormalizer lookupNormalizer,
            IDbContextProvider<TurisGoDbContext> dbContextProvider)
        {
            _usuarioRepository = usuarioRepository;
            _userManager = userManager;
            _identityUserRepository = identityUserRepository;
            _lookupNormalizer = lookupNormalizer;
            _dbContextProvider = dbContextProvider;
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


        // 1.3. Obtener el perfil del usuario actual
        [Authorize]
        public async Task<UsuarioDto> ObtenerPerfilActualAsync()
        {
            // Obtener el ID del IdentityUser del token JWT
            if (!CurrentUser.Id.HasValue)
            {
                throw new AbpAuthorizationException("Usuario no autenticado.");
            }

            var identityUserId = CurrentUser.Id.Value;

            // Buscar el usuario en AppUsuarios por IdentityUserId
            var usuario = await _usuarioRepository.
                FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);

            if (usuario == null)
            {
                throw new EntityNotFoundException(
                    typeof(Usuario),
                    $"No se encontró el perfil del usuario con IdentityUserId: {identityUserId}"
                );
            }

            // Verificar que el usuario esté activo
            if (!usuario.EstaActivo)
            {
                throw new BusinessException("El usuario no está activo.");
            }

            return ObjectMapper.Map<Usuario, UsuarioDto>(usuario);
        }

        // 1.3. Actualizar datos de perfil (nombre, email, foto)
        [Authorize]
        public async Task<UsuarioDto> ActualizarPerfilAsync(ActualizarPerfilDto input)
        {
            if (!CurrentUser.Id.HasValue)
            {
                throw new AbpAuthorizationException("Usuario no autenticado.");
            }

            var identityUserId = CurrentUser.Id.Value;
            var usuario = await _usuarioRepository
                .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);

            if (usuario == null)
            {
                throw new EntityNotFoundException(
                    typeof(Usuario),
                    $"No se encontró el perfil del usuario con IdentityUserId: {identityUserId}"
                );
            }

            // Variable para saber si hay algo que actualizar
            bool hayActualizaciones = false;

            // Solo actualizar nombre si viene informado y no está vacío
            if (!string.IsNullOrWhiteSpace(input.NombreCompleto))
            {
                usuario.SetNombreCompleto(input.NombreCompleto.Trim());
                hayActualizaciones = true;
            }

            // Solo actualizar email si viene informado y no está vacío
            if (!string.IsNullOrWhiteSpace(input.Email) && input.Email != usuario.Email)
            {
                var emailTrimmed = input.Email.Trim();

                // Validar que el email no esté en uso
                var emailEnUso = await _usuarioRepository
                    .AnyAsync(u => u.Email == emailTrimmed && u.Id != usuario.Id);

                if (emailEnUso)
                {
                    throw new BusinessException(
                        $"El correo electrónico '{emailTrimmed}' ya está en uso");
                }

                // Verificar en AbpUsers
                var normalizedEmail = _lookupNormalizer.NormalizeEmail(emailTrimmed);
                var usuarioConEmail = await _identityUserRepository
                    .FindByNormalizedEmailAsync(normalizedEmail);

                if (usuarioConEmail != null && usuarioConEmail.Id != identityUserId)
                {
                    throw new BusinessException(
                        $"El correo electrónico '{emailTrimmed}' ya está en uso");
                }

                // Actualizar email
                usuario.SetEmail(emailTrimmed);

                // Actualizar en AbpUsers
                var identityUser = await _userManager.GetByIdAsync(identityUserId);
                var setEmailResult = await _userManager.SetEmailAsync(identityUser, emailTrimmed);

                if (!setEmailResult.Succeeded)
                {
                    var errors = string.Join(", ", setEmailResult.Errors.Select(e => e.Description));
                    throw new UserFriendlyException(
                        $"Error al actualizar el correo electrónico: {errors}"
                    );
                }

                await _userManager.UpdateAsync(identityUser);
                hayActualizaciones = true;
            }

            // Para la foto: permitir actualizar incluso si es null (para eliminar la foto)
            // Pero solo si es diferente del valor actual
            if (input.FotoPerfilUrl != usuario.FotoPerfilUrl)
            {
                usuario.SetFotoPerfil(input.FotoPerfilUrl);
                hayActualizaciones = true;
            }

            // Solo guardar si hubo cambios
            if (hayActualizaciones)
            {
                await _usuarioRepository.UpdateAsync(usuario, autoSave: true);
            }

            return ObjectMapper.Map<Usuario, UsuarioDto>(usuario);
        }

        // 1.3. Actualizar preferencias de notificación
        [Authorize]
        public async Task<UsuarioDto> ActualizarPreferenciasAsync(ActualizarPreferenciasDto input) 
        {
            // Obtener el usuario actual
            if (!CurrentUser.Id.HasValue)
            {
                throw new AbpAuthorizationException("Usuario no autenticado.");
            }

            var identityUserId = CurrentUser.Id.Value;

            var usuario = await _usuarioRepository
                .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);

            if (usuario == null)
            {
                throw new EntityNotFoundException(
                    typeof(Usuario),
                    $"No se encontró el perfil del usuario con IdentityUserId: {identityUserId}"
                );
            }

            // Crear nuevo Value Object de preferencias
            var nuevasPreferencias = new PreferenciasNotificacion(
                input.RecibirEnPantalla,
                input.RecibirPorEmail,
                input.Frecuencia
            );

            // Actualizar las preferencias del usuario
            usuario.ActualizarPreferencias(nuevasPreferencias);

            await _usuarioRepository.UpdateAsync(usuario, autoSave: true);

            return ObjectMapper.Map<Usuario, UsuarioDto>(usuario);
        }

        // 1.4. Cambiar contraseña

        [Authorize]
        public async Task CambiarPasswordAsync(CambiarPasswordDto input)
        {

            // Validar que el usuario esté autenticado
            if (!CurrentUser.Id.HasValue)
            {
                throw new AbpAuthorizationException("Usuario no autenticado.");
            }

            var identityUserId = CurrentUser.Id.Value;

            // Verificar que el usduario exista en AbpUsers
            var usuario = await _usuarioRepository
                .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);

            if (usuario == null)
            {
                throw new EntityNotFoundException(
                    typeof(Usuario),
                    $"No se encontró el perfil del usuario con IdentityUserId: {identityUserId}"
                );
            }

            // Verificar que el usuario este activo
            if (!usuario.EstaActivo)
            {
                throw new BusinessException("El usuario no está activo.");
            }

            // Obtener el usuario de IdentityUser
            var identityUser = await _userManager.GetByIdAsync(identityUserId);

            var esPasswordValido = await _userManager.CheckPasswordAsync(
                identityUser,
                input.PasswordActual
            );

            if (!esPasswordValido)
            {
                throw new BusinessException("La contraseña actual es incorrecta.");
            }

            // Cambiar la contraseña
            var resultado = await _userManager.ChangePasswordAsync(
                identityUser,
                input.PasswordActual,
                input.NuevoPassword
            );

            if (!resultado.Succeeded)
            {
                var errores = string.Join(", ", resultado.Errors.Select(e => e.Description));
                throw new UserFriendlyException(
                    $"Error al cambiar la contraseña: {errores}"
                );
            }

            // Actualizar el usuario en IdentityUser
            await _userManager.UpdateAsync(identityUser);
        }

    }
}
