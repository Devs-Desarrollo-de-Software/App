import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { UsuarioService, PerfilPublicoDto } from 'src/app/proxy/usuarios';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-perfil-publico',
  standalone: true,
  imports: [CommonModule, FormsModule, CoreModule],
  templateUrl: './perfil-publico.component.html',
  styleUrls: ['./perfil-publico.component.scss'],
})
export class PerfilPublicoComponent implements OnInit {
  private readonly usuarioService = inject(UsuarioService);
  private readonly toaster = inject(ToasterService);

  perfil: PerfilPublicoDto | null = null;
  loading: boolean = false;
  nombreUsuarioBusqueda: string = '';
  mensajeError: string = '';

  ngOnInit(): void {}

  buscarPerfil(): void {
    const nombreUsuario = this.nombreUsuarioBusqueda.trim();
    console.log('buscarPerfil llamado con:', nombreUsuario);

    if (!nombreUsuario) {
      this.toaster.warn('Debe ingresar un nombre de usuario');
      return;
    }

    this.loading = true;
    this.perfil = null;
    this.mensajeError = '';

    this.usuarioService
      .obtenerPerfilPublico(nombreUsuario)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: perfil => {
          if (perfil) {
            this.perfil = perfil;
          } else {
            this.mensajeError = `No se encontró el usuario "${nombreUsuario}"`;
          }
        },
        error: () => {
          this.mensajeError = 'Error al buscar el usuario. Intente nuevamente.';
        },
      });
  }

  limpiarBusqueda(): void {
    this.nombreUsuarioBusqueda = '';
    this.perfil = null;
    this.mensajeError = '';
  }

  onSubmit(): void {
    this.buscarPerfil();
  }
}
