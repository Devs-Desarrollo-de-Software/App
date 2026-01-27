import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CoreModule } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { UsuarioService, UsuarioDto, TipoRol, FrecuenciaNotificacion } from 'src/app/proxy/usuarios';
import { finalize } from 'rxjs/operators';

// Componente que muestra el perfil del usuario autenticado
// Permite visualizar información personal y navegar a opciones de edición
@Component({
  selector: 'app-perfil-usuario',
  standalone: true,
  imports: [CommonModule, CoreModule],
  templateUrl: './perfil-usuario.component.html',
  styleUrls: ['./perfil-usuario.component.scss'],
})
export class PerfilUsuarioComponent implements OnInit {
  // Servicios inyectados
  private readonly usuarioService = inject(UsuarioService);
  private readonly toaster = inject(ToasterService);
  private readonly router = inject(Router);

  // Datos del usuario autenticado
  usuario: UsuarioDto | null = null;
  loading: boolean = false;

  // Enums para usar en el template
  TipoRol = TipoRol;
  FrecuenciaNotificacion = FrecuenciaNotificacion;

  ngOnInit(): void {
    this.cargarPerfil();
  }

  cargarPerfil(): void {
    this.loading = true;

    this.usuarioService
      .obtenerPerfilActual()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (usuario) => {
          this.usuario = usuario;
        },
        error: (err) => {
          console.error('Error al cargar el perfil:', err);
          this.toaster.error('Error al cargar el perfil del usuario');
        },
      });
  }

  editarPerfil(): void {
    this.router.navigate(['/usuarios/editar']);
  }

  cambiarPassword(): void {
    this.router.navigate(['/usuarios/cambiar-password']);
  }

  eliminarCuenta(): void {
    this.router.navigate(['/usuarios/eliminar-cuenta']);
  }

  getRolTexto(rol: TipoRol): string {
    return rol === TipoRol.Administrador ? 'Administrador' : 'Usuario';
  }

  getFrecuenciaTexto(frecuencia: FrecuenciaNotificacion): string {
    return frecuencia === FrecuenciaNotificacion.Inmediata ? 'Inmediata' : 'Semanal';
  }
}
