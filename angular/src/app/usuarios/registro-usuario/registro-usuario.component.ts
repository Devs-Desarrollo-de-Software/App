import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CoreModule } from '@abp/ng.core';
import { ToasterService, ConfirmationService } from '@abp/ng.theme.shared';
import { UsuarioService, CrearUsuarioDto, TipoRol } from 'src/app/proxy/usuarios';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-registro-usuario',
  standalone: true,
  imports: [CommonModule, FormsModule, CoreModule],
  templateUrl: './registro-usuario.component.html',
  styleUrls: ['./registro-usuario.component.scss'],
})
export class RegistroUsuarioComponent {
  private readonly usuarioService = inject(UsuarioService);
  private readonly toaster = inject(ToasterService);
  private readonly router = inject(Router);
  private readonly confirmation = inject(ConfirmationService);

  loading: boolean = false;
  mostrarPassword: boolean = false;

  // Modelo del formulario
  usuario: CrearUsuarioDto = {
    nombreCompleto: '',
    nombreUsuario: '',
    email: '',
    password: '',
    rol: TipoRol.Usuario,
    fotoPerfilUrl: undefined,
  };

  // Enum para el template
  TipoRol = TipoRol;

  // Opciones de roles
  roles = [
    { valor: TipoRol.Usuario, etiqueta: 'Usuario' },
    { valor: TipoRol.Administrador, etiqueta: 'Administrador' },
  ];

  registrarUsuario(): void {
    // Validaciones básicas
    if (!this.validarFormulario()) {
      return;
    }

    this.loading = true;

    // Limpiar foto si está vacía
    const input = {
      ...this.usuario,
      fotoPerfilUrl: this.usuario.fotoPerfilUrl?.trim() || undefined,
    };

    this.usuarioService
      .registrarUsuario(input)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: () => {
          this.toaster.success('Usuario registrado exitosamente');
          this.router.navigate(['/usuarios/lista']);
        },
        error: (err) => {
          console.error('Error al registrar usuario:', err);

          // Manejar errores específicos
          const errorCode = err?.error?.error?.code || '';
          const errorMessage = err?.error?.error?.message || '';

          // Si es un usuario inactivo, mostrar modal de confirmación
          if (errorCode === 'UsuarioInactivo') {
            this.mostrarModalReactivacion(input);
          } else if (errorMessage.includes('nombre de usuario') && errorMessage.includes('en uso')) {
            this.toaster.error('El nombre de usuario ya está en uso');
          } else if (errorMessage.includes('correo electrónico') && errorMessage.includes('en uso')) {
            this.toaster.error('El correo electrónico ya está en uso');
          } else if (errorMessage.includes('contraseña')) {
            this.toaster.error('La contraseña no cumple con los requisitos de seguridad');
          } else {
            this.toaster.error('Error al registrar el usuario. Por favor, intente nuevamente.');
          }
        },
      });
  }

  validarFormulario(): boolean {
    if (!this.usuario.nombreCompleto?.trim()) {
      this.toaster.warn('El nombre completo es obligatorio');
      return false;
    }

    if (!this.usuario.nombreUsuario?.trim()) {
      this.toaster.warn('El nombre de usuario es obligatorio');
      return false;
    }

    if (!this.usuario.email?.trim()) {
      this.toaster.warn('El correo electrónico es obligatorio');
      return false;
    }

    // Validación básica de email
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(this.usuario.email)) {
      this.toaster.warn('El formato del correo electrónico no es válido');
      return false;
    }

    if (!this.usuario.password || this.usuario.password.length < 6) {
      this.toaster.warn('La contraseña debe tener al menos 6 caracteres');
      return false;
    }

    return true;
  }

  toggleMostrarPassword(): void {
    this.mostrarPassword = !this.mostrarPassword;
  }

  cancelar(): void {
    this.router.navigate(['/usuarios/lista']);
  }

  limpiarFormulario(): void {
    this.usuario = {
      nombreCompleto: '',
      nombreUsuario: '',
      email: '',
      password: '',
      rol: TipoRol.Usuario,
      fotoPerfilUrl: undefined,
    };
  }

  private mostrarModalReactivacion(input: CrearUsuarioDto): void {
    this.confirmation
      .warn(
        `Existe un usuario inactivo con el nombre de usuario '${input.nombreUsuario}'. ¿Deseas reactivarlo con los nuevos datos?`,
        'Reactivar Usuario'
      )
      .subscribe((status) => {
        if (status === 'confirm') {
          this.reactivarUsuario(input);
        }
      });
  }

  private reactivarUsuario(input: CrearUsuarioDto): void {
    this.loading = true;

    this.usuarioService
      .reactivarUsuario(input)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: () => {
          this.toaster.success('Usuario reactivado exitosamente');
          this.router.navigate(['/usuarios/lista']);
        },
        error: (err) => {
          console.error('Error al reactivar usuario:', err);
          this.toaster.error('Error al reactivar el usuario. Por favor, intente nuevamente.');
        },
      });
  }
}
