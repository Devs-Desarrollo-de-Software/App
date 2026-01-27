import { authGuard, permissionGuard } from '@abp/ng.core';
import { Routes } from '@angular/router';
import { CustomLayoutComponent } from './layout/custom-layout/custom-layout.component';

export const APP_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./home/home.component').then(c => c.HomeComponent),
  },
  {
    path: 'account/manage',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./usuarios/mi-cuenta/mi-cuenta.component').then(c => c.MiCuentaComponent),
  },
  {
    path: 'configuracion',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./configuracion/configuracion.component').then(c => c.ConfiguracionComponent),
  },
  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(c => c.createRoutes()),
  },
  {
    path: 'identity',
    loadChildren: () => import('@abp/ng.identity').then(c => c.createRoutes()),
  },
  {
    path: 'setting-management',
    loadChildren: () => import('@abp/ng.setting-management').then(c => c.createRoutes()),
  },
  {
    path: 'destinos',
    loadChildren: () => import('./destinos/destinos.routes').then(m => m.DESTINOS_ROUTES),
  },
  {
    path: 'mis-calificaciones',
    loadChildren: () =>
      import('./calificaciones/calificaciones.routes').then(m => m.CALIFICACIONES_ROUTES),
  },
  {
    path: 'usuarios',
    loadChildren: () => import('./usuarios/usuarios.routes').then(m => m.USUARIOS_ROUTES),
  },
];
