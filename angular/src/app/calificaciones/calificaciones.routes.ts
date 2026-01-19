import { Routes } from '@angular/router';
import { authGuard } from '@abp/ng.core';

/**
 * Rutas del módulo de Calificaciones
 * Utiliza lazy loading para optimizar la carga inicial de la aplicación
 * El authGuard asegura que solo usuarios autenticados puedan acceder
 */
export const CALIFICACIONES_ROUTES: Routes = [
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./mis-calificaciones-list/mis-calificaciones-list.component').then(
        (c) => c.MisCalificacionesListComponent
      ),
  },
];
