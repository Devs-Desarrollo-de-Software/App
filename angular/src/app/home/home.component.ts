import { Component, inject } from '@angular/core';
import { AuthService } from '@abp/ng.core';

// Componente de la página de inicio/home
// Muestra contenido diferente según el estado de autenticación del usuario
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  imports: []
})
export class HomeComponent {
  // Servicio de autenticación de ABP
  private authService = inject(AuthService);

  // Verifica si el usuario está autenticado
  get hasLoggedIn(): boolean {
    return this.authService.isAuthenticated
  }

  // Redirige al usuario a la página de login
  login() {
    this.authService.navigateToLogin();
  }
}
