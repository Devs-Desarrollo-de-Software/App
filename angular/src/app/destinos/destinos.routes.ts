import { Routes } from '@angular/router';
import { authGuard } from '@abp/ng.core';

/**
 * Rutas del módulo de Destinations
 * Utiliza lazy loading para optimizar la carga inicial de la aplicación
 * El authGuard asegura que solo usuarios autenticados puedan acceder
 */
export const DESTINOS_ROUTES: Routes = [
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./destinos-list/destinos-list').then(
        c => c.DestinosList
      ),
  },
];