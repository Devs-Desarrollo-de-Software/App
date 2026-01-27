import { Component, Output, EventEmitter, HostListener, ElementRef, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ConfigStateService, AuthService } from '@abp/ng.core';
import { UsuarioService, UsuarioDto } from '../../proxy/usuarios';

@Component({
  selector: 'app-custom-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './custom-navbar.component.html',
  styleUrls: ['./custom-navbar.component.scss'],
})
export class CustomNavbarComponent implements OnInit {
  @Output() toggleSidebar = new EventEmitter<void>();
  showUserMenu = false;
  userProfile: UsuarioDto | null = null;

  private usuarioService = inject(UsuarioService);

  constructor(
    private configState: ConfigStateService,
    private authService: AuthService,
    private elementRef: ElementRef,
  ) {}

  ngOnInit() {
    if (this.isAuthenticated) {
      this.cargarPerfil();
    }
  }

  cargarPerfil() {
    this.usuarioService.obtenerPerfilActual().subscribe({
      next: (perfil) => {
        this.userProfile = perfil;
      },
      error: (err) => {
        console.error('Error al cargar perfil:', err);
      }
    });
  }

  get currentUser() {
    return this.configState.getOne('currentUser');
  }

  get userName(): string {
    return this.currentUser?.userName || 'Usuario';
  }

  get userEmail(): string {
    return this.currentUser?.email || '';
  }

  get isAuthenticated(): boolean {
    return this.currentUser?.isAuthenticated || false;
  }

  get userPhoto(): string {
    return this.userProfile?.fotoPerfilUrl || 'https://cdn-icons-png.flaticon.com/512/12225/12225881.png';
  }

  toggleUserMenu(event: Event) {
    event.stopPropagation();
    this.showUserMenu = !this.showUserMenu;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event) {
    const clickedInside = this.elementRef.nativeElement.contains(event.target);
    if (!clickedInside && this.showUserMenu) {
      this.showUserMenu = false;
    }
  }

  logout() {
    this.authService.logout();
    this.showUserMenu = false;
  }

  onToggleSidebar() {
    this.toggleSidebar.emit();
  }
}
