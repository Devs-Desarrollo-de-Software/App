import { RoutesService, eLayoutType } from '@abp/ng.core';
import { inject, provideAppInitializer } from '@angular/core';
import { eAccountRouteNames } from '@abp/ng.account/config';

// Proveedor de configuración de rutas de la aplicación
// Se ejecuta al inicializar la app para configurar el menú de navegación
export const APP_ROUTE_PROVIDER = [
  provideAppInitializer(() => {
    configureRoutes();
  }),
];

// Configura las rutas y el menú de navegación de la aplicación
function configureRoutes() {
  const routes = inject(RoutesService);

  // Remover la ruta de registro de ABP Account para deshabilitar el auto-registro
  // Solo los administradores pueden crear usuarios desde el panel de administración
  routes.remove([eAccountRouteNames.Register]);

  routes.add([
    {
      path: '/',
      name: '::Menu:Home',
      iconClass: 'fas fa-home',
      order: 1,
      layout: eLayoutType.application,
    },
    {
      path: '/destinos',
      name: '::Destinations',
      iconClass: 'fas fa-map-marker-alt',
      order: 2,
      layout: eLayoutType.application,
    },
    {
      path: '/mis-calificaciones',
      name: '::MyReviews',
      iconClass: 'fas fa-star',
      order: 3,
      layout: eLayoutType.application,
    },
    {
      path: '/usuarios/perfil-publico',
      name: '::SearchUser',
      iconClass: 'fas fa-search',
      order: 4,
      layout: eLayoutType.application,
    },
    {
      path: '/usuarios/lista',
      name: '::UserManagement',
      iconClass: 'fas fa-users-cog',
      order: 5,
      layout: eLayoutType.application,
      requiredPolicy: 'admin',
    },
  ]);
}
