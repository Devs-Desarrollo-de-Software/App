import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ConfigStateService } from '@abp/ng.core';

interface MenuItem {
  label: string;
  icon: string;
  route: string;
  requiredRole?: string;
}

@Component({
  selector: 'app-custom-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './custom-sidebar.component.html',
  styleUrls: ['./custom-sidebar.component.scss']
})
export class CustomSidebarComponent {
  @Input() collapsed = false;

  menuItems: MenuItem[] = [
    { label: 'Home', icon: 'fas fa-home', route: '/' },
    { label: 'Destinos', icon: 'fas fa-map-marker-alt', route: '/destinos' },
    { label: 'Mis Calificaciones', icon: 'fas fa-star', route: '/mis-calificaciones' },
    { label: 'Buscar Usuario', icon: 'fas fa-search', route: '/usuarios/perfil-publico' },
    { label: 'Gestión Usuarios', icon: 'fas fa-users-cog', route: '/usuarios/lista', requiredRole: 'admin' },
  ];

  constructor(private configState: ConfigStateService) {}

  get currentUser() {
    return this.configState.getOne('currentUser');
  }

  get userRoles(): string[] {
    return this.currentUser?.roles || [];
  }

  canShowItem(item: MenuItem): boolean {
    if (!item.requiredRole) {
      return true;
    }
    return this.userRoles.includes(item.requiredRole) || this.userRoles.includes('Admin');
  }
}
