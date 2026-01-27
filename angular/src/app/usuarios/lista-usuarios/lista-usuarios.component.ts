import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CoreModule, PagedResultDto, ConfigStateService } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { UsuarioService } from '../../proxy/usuarios/usuario.service';
import { UsuarioDto, TipoRol, ObtenerUsuariosInput } from '../../proxy/usuarios/models';
import { finalize } from 'rxjs/operators';
import { ConfirmationService } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-lista-usuarios',
  standalone: true,
  imports: [CommonModule, CoreModule, FormsModule],
  templateUrl: './lista-usuarios.component.html',
  styleUrls: ['./lista-usuarios.component.scss'],
})
export class ListaUsuariosComponent implements OnInit {
  private readonly toaster = inject(ToasterService);
  private readonly router = inject(Router);
  private readonly usuarioService = inject(UsuarioService);
  private readonly confirmationService = inject(ConfirmationService);
  private readonly configState = inject(ConfigStateService);

  usuarios: UsuarioDto[] = [];
  loading: boolean = false;
  totalCount: number = 0;
  currentUserId: string = '';

  // Filtros
  filtro: string = '';
  rolFiltro?: TipoRol;
  estadoFiltro?: boolean;

  // Paginación
  pageSize: number = 10;
  currentPage: number = 1;
  skipCount: number = 0;

  // Enums para el template
  TipoRol = TipoRol;

  ngOnInit(): void {
    // Obtener el ID del usuario actual
    const currentUser = this.configState.getOne('currentUser');
    this.currentUserId = currentUser?.id || '';
    this.cargarUsuarios();
  }

  cargarUsuarios(): void {
    this.loading = true;

    const input: ObtenerUsuariosInput = {
      filtro: this.filtro || '',
      rol: this.rolFiltro,
      estaActivo: this.estadoFiltro,
      skipCount: this.skipCount,
      maxResultCount: this.pageSize,
      sorting: 'nombreCompleto',
    };

    this.usuarioService
      .obtenerTodosUsuarios(input)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (result: PagedResultDto<UsuarioDto>) => {
          this.usuarios = result.items;
          this.totalCount = result.totalCount;
        },
        error: (error) => {
          this.toaster.error('::ErrorAlCargarUsuarios', '::Error');
          console.error('Error al cargar usuarios:', error);
        },
      });
  }

  registrarNuevoUsuario(): void {
    this.router.navigate(['/usuarios/registro']);
  }

  editarUsuario(id: string): void {
    this.router.navigate(['/usuarios/editar', id]);
  }

  eliminarUsuario(usuario: UsuarioDto): void {
    // Verificar si el usuario intenta eliminarse a sí mismo
    if (usuario.id === this.currentUserId) {
      this.toaster.warn('::CannotDeleteYourself', '::Warning');
      return;
    }

    this.confirmationService
      .warn(
        `¿Estás seguro de que deseas eliminar al usuario "${usuario.nombreCompleto}"?`,
        'Confirmar eliminación'
      )
      .subscribe((status) => {
        if (status === 'confirm') {
          this.loading = true;
          this.usuarioService
            .eliminarUsuario(usuario.id)
            .pipe(finalize(() => (this.loading = false)))
            .subscribe({
              next: () => {
                this.toaster.success('::UsuarioEliminadoExitosamente', '::Exito');
                this.cargarUsuarios();
              },
              error: (error) => {
                this.toaster.error('::ErrorAlEliminarUsuario', '::Error');
                console.error('Error al eliminar usuario:', error);
              },
            });
        }
      });
  }

  aplicarFiltros(): void {
    this.currentPage = 1;
    this.skipCount = 0;
    this.cargarUsuarios();
  }

  limpiarFiltros(): void {
    this.filtro = '';
    this.rolFiltro = undefined;
    this.estadoFiltro = undefined;
    this.aplicarFiltros();
  }

  cambiarPagina(page: number): void {
    this.currentPage = page;
    this.skipCount = (page - 1) * this.pageSize;
    this.cargarUsuarios();
  }

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize);
  }

  get paginasTotales(): number[] {
    const pages = [];
    for (let i = 1; i <= this.totalPages; i++) {
      pages.push(i);
    }
    return pages;
  }

  getRolBadgeClass(rol: TipoRol): string {
    return rol === TipoRol.Administrador ? 'badge-admin' : 'badge-user';
  }

  getRolTexto(rol: TipoRol): string {
    return rol === TipoRol.Administrador ? 'Administrador' : 'Usuario';
  }
}
