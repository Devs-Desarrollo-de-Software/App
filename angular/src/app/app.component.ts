import { Component } from '@angular/core';
import { LoaderBarComponent } from '@abp/ng.theme.shared';
import { CustomLayoutComponent } from './layout/custom-layout/custom-layout.component';

// Componente raíz de la aplicación TurisGo
// Configura el layout personalizado y la barra de carga de ABP
@Component({
  selector: 'app-root',
  template: `
    <abp-loader-bar />
    <app-custom-layout />
  `,
  imports: [LoaderBarComponent, CustomLayoutComponent],
})
export class AppComponent {}
