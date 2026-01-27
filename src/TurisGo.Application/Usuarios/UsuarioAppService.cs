using Abp.Runtime.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using TurisGo.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Microsoft.EntityFrameworkCore;


namespace TurisGo.Usuarios
{
    // Servicio de aplicación para la gestión de usuarios
    // Implementa operaciones CRUD y gestión de perfiles de usuario
    // Requiere autorización para todos los métodos excepto los marcados con [AllowAnonymous]
    [Authorize]
    public class UsuarioAppService : ApplicationService, IUsuarioAppService
    {
        // Repositorio para la entidad Usuario (tabla AppUsuarios)
        private readonly Volo.Abp.Domain.Repositories.IRepository<Usuario, Guid> _usuarioRepository;

        // Gestor de usuarios de Identity de ABP (tabla AbpUsers)
        private readonly IdentityUserManager _userManager;

        // Repositorio de usuarios de Identity para consultas
        private readonly IIdentityUserRepository _identityUserRepository;

        // Normalizador para nombres de usuario y emails (para búsquedas case-insensitive)
        private readonly ILookupNormalizer _lookupNormalizer;

        // Proveedor de contexto de base de datos para operaciones avanzadas
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

        // ==================== OPERACIONES DE USUARIO ====================

        // Registra un nuevo usuario en el sistema (solo administradores)
        // Crea el usuario tanto en AbpUsers (autenticación) como en AppUsuarios (datos de negocio)
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<UsuarioDto> RegistrarUsuarioAsync(CrearUsuarioDto input)
        {
            // Validar que el nombre de usuario no exista entre usuarios ACTIVOS
            var usuarioExistente = await _usuarioRepository
                .FirstOrDefaultAsync(u => u.NombreUsuario == input.NombreUsuario);

            if (usuarioExistente != null && usuarioExistente.EstaActivo)
            {
                throw new BusinessException($"El nombre de usuario '{input.NombreUsuario}' ya está en uso");
            }

            // Si existe un usuario inactivo con ese username, lanzar excepción específica
            if (usuarioExistente != null && !usuarioExistente.EstaActivo)
            {
                throw new BusinessException(
                    "UsuarioInactivo",
                    $"Existe un usuario inactivo con el nombre de usuario '{input.NombreUsuario}'. ¿Deseas reactivarlo?"
                );
            }

            // Validar que el email no exista entre usuarios ACTIVOS
            var emailExistente = await _usuarioRepository
                .FirstOrDefaultAsync(u => u.Email == input.Email);

            if (emailExistente != null && emailExistente.EstaActivo)
            {
                throw new BusinessException($"El correo electrónico '{input.Email}' ya está en uso");
            }

            // Validar que el nombre de usuario no exista en IdentityUser activos
            var normalizedUserName = _lookupNormalizer.NormalizeName(input.NombreUsuario);
            var existeIdentityUserActivo = await _identityUserRepository
                .FindByNormalizedUserNameAsync(normalizedUserName);

            if (existeIdentityUserActivo != null && existeIdentityUserActivo.IsActive)
            {
                throw new UserFriendlyException($"El nombre de usuario '{input.NombreUsuario}' ya está en uso");
            }

            // Crear el usuario en la tabla de Identity de ABP (AbpUsers)
            // Esto permite el login y autenticación
            var identityUser = new IdentityUser(
                GuidGenerator.Create(),
                input.NombreUsuario,
                input.Email,
                CurrentTenant.Id
            );

            // Asignar la contraseña con las validaciones de ABP Identity
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

            // Asignar el rol correspondiente en ABP Identity
            var roleName =input.Rol == TipoRol.Administrador ? "Admin" : "User";
            await _userManager.AddToRoleAsync(identityUser, roleName);

            // Crear el usuario en la tabla de negocio (AppUsuarios)
            // Esto almacena datos adicionales específicos de la aplicación
            var usuario = new Usuario(
                GuidGenerator.Create(),
                input.NombreCompleto,
                input.NombreUsuario,
                identityUser.Id,    // Conexión con AbpUsers mediante IdentityUserId
                input.Email,
                input.Rol,
                input.FotoPerfilUrl
            );

            // Guardar el usuario en la base de datos
            await _usuarioRepository.InsertAsync(usuario, autoSave: true);

            // Mapear la entidad a DTO y retornar
            return ObjectMapper.Map<Usuario, UsuarioDto>(usuario);
        }

        // Obtiene el perfil del usuario autenticado actualmente
        // Si el usuario no existe en AppUsuarios (usuarios de seed), lo crea automáticamente
        [Authorize]
        [HttpGet]
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

            // Si no existe en AppUsuarios, crearlo automáticamente
            // Esto maneja el caso de usuarios creados directamente en AbpUsers (como usuarios de seed)
            if (usuario == null)
            {
                var identityUser = await _userManager.GetByIdAsync(identityUserId);

                // Determinar el rol según los roles asignados en ABP Identity
                var roles = await _userManager.GetRolesAsync(identityUser);
                var tipoRol = roles.Contains("admin") ? TipoRol.Administrador : TipoRol.Usuario;

                // Crear el usuario en AppUsuarios
                usuario = new Usuario(
                    GuidGenerator.Create(),
                    identityUser.Name ?? identityUser.UserName ?? "Usuario",
                    identityUser.UserName ?? "user",
                    identityUserId,
                    identityUser.Email ?? "",
                    tipoRol,
                    null
                );

                await _usuarioRepository.InsertAsync(usuario, autoSave: true);
            }

            // Verificar que el usuario esté activo
            if (!usuario.EstaActivo)
            {
                throw new BusinessException("El usuario no está activo.");
            }

            return ObjectMapper.Map<Usuario, UsuarioDto>(usuario);
        }

        // Actualiza el perfil del usuario autenticado
        // Permite modificar nombre completo, email y foto de perfil
        // Solo actualiza los campos que vienen informados
        [Authorize]
        [HttpPut]
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

        // Actualiza las preferencias de notificación del usuario autenticado
        [Authorize]
        [HttpPut]
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

        // Cambia la contraseña del usuario autenticado
        // Requiere la contraseña actual para verificación
        [Authorize]
        [HttpPost]
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

        // Obtiene el perfil público de un usuario por su nombre de usuario
        // Accesible sin autenticación para permitir ver perfiles de otros usuarios
        // Retorna null si el usuario no existe o no está activo (para evitar modal de error)
        [AllowAnonymous]
        [HttpGet]
        public async Task<PerfilPublicoDto?> ObtenerPerfilPublicoAsync(string nombreUsuario)
        {
            // Validar entrada
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                return null;
            }

            // Buscar por nombre de usuario
            var usuario = await _usuarioRepository
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

            // Si no existe o no está activo, retornar null
            if (usuario == null || !usuario.EstaActivo)
            {
                return null;
            }

            return ObjectMapper.Map<Usuario, PerfilPublicoDto>(usuario);
        }

        // ==================== GESTIÓN DE USUARIOS (ADMIN) ====================

        // Obtiene una lista paginada de todos los usuarios con filtros opcionales
        // Solo accesible para administradores
        // Permite filtrar por nombre, email, rol y estado activo
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<PagedResultDto<UsuarioDto>> ObtenerTodosUsuariosAsync(ObtenerUsuariosInput input)
        {
            var query = await _usuarioRepository.GetQueryableAsync();

            // Aplicar filtro de búsqueda por nombre o email
            if (!string.IsNullOrWhiteSpace(input.Filtro))
            {
                var filtro = input.Filtro.Trim().ToLower();
                query = query.Where(u =>
                    u.NombreCompleto.ToLower().Contains(filtro) ||
                    u.NombreUsuario.ToLower().Contains(filtro) ||
                    u.Email.ToLower().Contains(filtro)
                );
            }

            // Filtrar por rol
            if (input.Rol.HasValue)
            {
                query = query.Where(u => u.Rol == input.Rol.Value);
            }

            // Filtrar por estado activo
            if (input.EstaActivo.HasValue)
            {
                query = query.Where(u => u.EstaActivo == input.EstaActivo.Value);
            }

            // Obtener total de registros
            var totalCount = await AsyncExecuter.CountAsync(query);

            // Aplicar ordenamiento
            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                query = query.OrderBy(input.Sorting);
            }
            else
            {
                // Ordenamiento por defecto: por nombre
                query = query.OrderBy(u => u.NombreCompleto);
            }

            // Aplicar paginación
            query = query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            // Ejecutar query
            var usuarios = await AsyncExecuter.ToListAsync(query);

            // Mapear a DTOs
            var usuarioDtos = ObjectMapper.Map<List<Usuario>, List<UsuarioDto>>(usuarios);

            return new PagedResultDto<UsuarioDto>(
                totalCount,
                usuarioDtos
            );
        }

        // Obtiene un usuario específico por su ID (solo administradores)
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/app/usuario/{id}")]
        public async Task<UsuarioDto> ObtenerUsuarioPorIdAsync(Guid id)
        {
            var usuario = await _usuarioRepository.FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                throw new UserFriendlyException("Usuario no encontrado");
            }

            return ObjectMapper.Map<Usuario, UsuarioDto>(usuario);
        }

        // Actualiza los datos de un usuario específico (solo administradores)
        // El admin puede modificar todos los campos excepto el nombre de usuario
        // Actualiza tanto en AppUsuarios como en AbpUsers
        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("api/app/usuario/{id}")]
        public async Task<UsuarioDto> ActualizarUsuarioAsync(Guid id, ActualizarUsuarioDto input)
        {
            var usuario = await _usuarioRepository.FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                throw new UserFriendlyException("Usuario no encontrado");
            }

            bool hayActualizaciones = false;

            // Actualizar nombre completo
            if (!string.IsNullOrWhiteSpace(input.NombreCompleto) &&
                input.NombreCompleto != usuario.NombreCompleto)
            {
                usuario.SetNombreCompleto(input.NombreCompleto.Trim());
                hayActualizaciones = true;
            }

            // Actualizar email
            if (!string.IsNullOrWhiteSpace(input.Email) && input.Email != usuario.Email)
            {
                var emailTrimmed = input.Email.Trim();

                // Validar que el email no esté en uso por otro usuario
                var emailEnUso = await _usuarioRepository
                    .AnyAsync(u => u.Email == emailTrimmed && u.Id != id);

                if (emailEnUso)
                {
                    throw new BusinessException(
                        $"El correo electrónico '{emailTrimmed}' ya está en uso");
                }

                // Verificar en AbpUsers
                var normalizedEmail = _lookupNormalizer.NormalizeEmail(emailTrimmed);
                var usuarioConEmail = await _identityUserRepository
                    .FindByNormalizedEmailAsync(normalizedEmail);

                if (usuarioConEmail != null && usuarioConEmail.Id != usuario.IdentityUserId)
                {
                    throw new BusinessException(
                        $"El correo electrónico '{emailTrimmed}' ya está en uso");
                }

                // Actualizar email
                usuario.SetEmail(emailTrimmed);

                // Actualizar en AbpUsers
                var identityUser = await _userManager.GetByIdAsync(usuario.IdentityUserId);
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

            // Actualizar foto de perfil
            if (input.FotoPerfilUrl != usuario.FotoPerfilUrl)
            {
                usuario.SetFotoPerfil(input.FotoPerfilUrl);
                hayActualizaciones = true;
            }

            // Actualizar rol
            if (input.Rol != usuario.Rol)
            {
                usuario.SetRol(input.Rol);

                // Actualizar rol en AbpUsers
                var identityUser = await _userManager.GetByIdAsync(usuario.IdentityUserId);
                var rolesActuales = await _userManager.GetRolesAsync(identityUser);

                // Remover roles actuales
                await _userManager.RemoveFromRolesAsync(identityUser, rolesActuales);

                // Asignar nuevo rol
                var nuevoRolNombre = input.Rol == TipoRol.Administrador ? "admin" : "User";
                await _userManager.AddToRoleAsync(identityUser, nuevoRolNombre);

                hayActualizaciones = true;
            }

            // Actualizar estado activo
            if (input.EstaActivo != usuario.EstaActivo)
            {
                if (input.EstaActivo)
                {
                    usuario.Activar();
                }
                else
                {
                    usuario.Desactivar();
                }

                // Actualizar en AbpUsers
                var identityUser = await _userManager.GetByIdAsync(usuario.IdentityUserId);
                identityUser.SetIsActive(input.EstaActivo);
                await _userManager.UpdateAsync(identityUser);

                hayActualizaciones = true;
            }

            // Solo guardar si hubo cambios
            if (hayActualizaciones)
            {
                await _usuarioRepository.UpdateAsync(usuario, autoSave: true);
            }

            return ObjectMapper.Map<Usuario, UsuarioDto>(usuario);
        }

        // Desactiva un usuario (solo administradores)
        // Soft delete: marca el usuario como inactivo en lugar de eliminarlo físicamente
        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("api/app/usuario/{id}")]
        public async Task EliminarUsuarioAsync(Guid id)
        {
            var usuario = await _usuarioRepository.FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                throw new UserFriendlyException("Usuario no encontrado");
            }

            // Verificar que el administrador no se esté eliminando a sí mismo
            if (CurrentUser.Id.HasValue && usuario.IdentityUserId == CurrentUser.Id.Value)
            {
                throw new BusinessException("No puedes eliminar tu propia cuenta. Usa la opción 'Eliminar Cuenta' desde tu perfil.");
            }

            // Verificar que el usuario no esté ya desactivado
            if (!usuario.EstaActivo)
            {
                throw new BusinessException("El usuario ya está desactivado.");
            }

            // Obtener el IdentityUser
            var identityUser = await _userManager.GetByIdAsync(usuario.IdentityUserId);

            // Desactivar el usuario (soft delete)
            usuario.Desactivar();
            identityUser.SetIsActive(false);

            // Guardar cambios
            await _userManager.UpdateAsync(identityUser);
            await _usuarioRepository.UpdateAsync(usuario, autoSave: true);
        }

        // Reactiva un usuario previamente desactivado y actualiza sus datos (solo administradores)
        // Permite reutilizar un nombre de usuario que estaba asociado a una cuenta inactiva
        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("api/app/usuario/reactivar")]
        public async Task<UsuarioDto> ReactivarUsuarioAsync(CrearUsuarioDto input)
        {
            // Buscar el usuario inactivo por nombre de usuario
            var usuario = await _usuarioRepository
                .FirstOrDefaultAsync(u => u.NombreUsuario == input.NombreUsuario);

            if (usuario == null)
            {
                throw new EntityNotFoundException(
                    typeof(Usuario),
                    $"No se encontró el usuario con nombre de usuario: {input.NombreUsuario}"
                );
            }

            if (usuario.EstaActivo)
            {
                throw new BusinessException("El usuario ya está activo.");
            }

            // Obtener el IdentityUser
            var identityUser = await _userManager.GetByIdAsync(usuario.IdentityUserId);

            // Reactivar el usuario
            usuario.Activar();
            identityUser.SetIsActive(true);

            // Actualizar datos del usuario
            usuario.SetNombreCompleto(input.NombreCompleto);
            usuario.SetEmail(input.Email);
            usuario.SetFotoPerfil(input.FotoPerfilUrl);
            usuario.SetRol(input.Rol);

            // Actualizar email en IdentityUser
            var setEmailResult = await _userManager.SetEmailAsync(identityUser, input.Email);
            if (!setEmailResult.Succeeded)
            {
                var errors = string.Join(", ", setEmailResult.Errors.Select(e => e.Description));
                throw new UserFriendlyException($"Error al actualizar el correo electrónico: {errors}");
            }

            // Actualizar contraseña
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
            var resetResult = await _userManager.ResetPasswordAsync(identityUser, resetToken, input.Password);
            if (!resetResult.Succeeded)
            {
                var errors = string.Join(", ", resetResult.Errors.Select(e => e.Description));
                throw new UserFriendlyException($"Error al actualizar la contraseña: {errors}");
            }

            // Actualizar roles
            var rolesActuales = await _userManager.GetRolesAsync(identityUser);
            await _userManager.RemoveFromRolesAsync(identityUser, rolesActuales);
            var nuevoRolNombre = input.Rol == TipoRol.Administrador ? "admin" : "User";
            await _userManager.AddToRoleAsync(identityUser, nuevoRolNombre);

            // Guardar cambios
            await _userManager.UpdateAsync(identityUser);
            await _usuarioRepository.UpdateAsync(usuario, autoSave: true);

            return ObjectMapper.Map<Usuario, UsuarioDto>(usuario);
        }
    }
}
