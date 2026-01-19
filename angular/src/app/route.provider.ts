import { RoutesService, eLayoutType } from '@abp/ng.core';
import { inject, provideAppInitializer } from '@angular/core';

export const APP_ROUTE_PROVIDER = [
  provideAppInitializer(() => {
    configureRoutes();
  }),
];

function configureRoutes() {
  const routes = inject(RoutesService);
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
      name: 'Destinos',
      iconClass: 'fas fa-map-marker-alt',
      order: 2,
      layout: eLayoutType.application,
    },
    {
      path: '/mis-calificaciones',
      name: 'Mis Calificaciones',
      iconClass: 'fas fa-star',
      order: 3,
      layout: eLayoutType.application,
    },
  ]);
}
