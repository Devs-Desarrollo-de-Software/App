import { Routes } from '@angular/router';
import { authGuard } from '@abp/ng.core';

/**
 * Rutas del módulo de Usuarios
 * Utiliza lazy loading para optimizar la carga inicial de la aplicación
 */
export const USUARIOS_ROUTES: Routes = [
  // 1.3 - Ver mi perfil (requiere autenticación)
  {
    path: 'perfil',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./perfil-usuario/perfil-usuario.component').then(
        (c) => c.PerfilUsuarioComponent
      ),
  },
  // 1.4 - Cambiar contraseña (requiere autenticación)
  {
    path: 'cambiar-password',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./cambiar-password/cambiar-password.component').then(
        (c) => c.CambiarPasswordComponent
      ),
  },
  // 1.5 - Mi cuenta (editar perfil y preferencias) (requiere autenticación)
  {
    path: 'mi-cuenta',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./mi-cuenta/mi-cuenta.component').then(
        (c) => c.MiCuentaComponent
      ),
  },
  // 1.6 - Ver perfil público (sin autenticación)
  {
    path: 'perfil-publico',
    loadComponent: () =>
      import('./perfil-publico/perfil-publico.component').then(
        (c) => c.PerfilPublicoComponent
      ),
  },
  // 1.1 - Registrar nuevo usuario (solo Admin - requiere autenticación)
  // TODO: Agregar guard de rol Admin cuando se implemente
  {
    path: 'registro',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./registro-usuario/registro-usuario.component').then(
        (c) => c.RegistroUsuarioComponent
      ),
  },
  // Lista de usuarios (solo Admin - requiere autenticación)
  // TODO: Agregar guard de rol Admin cuando se implemente
  {
    path: 'lista',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./lista-usuarios/lista-usuarios.component').then(
        (c) => c.ListaUsuariosComponent
      ),
  },
  // Editar usuario (solo Admin - requiere autenticación)
  // TODO: Agregar guard de rol Admin cuando se implemente
  {
    path: 'editar/:id',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./editar-usuario/editar-usuario.component').then(
        (c) => c.EditarUsuarioComponent
      ),
  },
  // Redirección por defecto al perfil
  {
    path: '',
    redirectTo: 'perfil',
    pathMatch: 'full',
  },
];
