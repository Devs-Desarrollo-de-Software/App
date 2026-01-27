import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ToasterService } from '@abp/ng.theme.shared';
import { LocalizationPipe } from '@abp/ng.core';
import { UsuarioService } from '../../proxy/usuarios/usuario.service';
import { CambiarPasswordDto } from '../../proxy/usuarios/models';
import { finalize } from 'rxjs/operators';
import { Router } from '@angular/router';

@Component({
  selector: 'app-cambiar-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, LocalizationPipe],
  templateUrl: './cambiar-password.component.html',
  styleUrl: './cambiar-password.component.scss'
})
export class CambiarPasswordComponent {
  private fb = inject(FormBuilder);
  private usuarioService = inject(UsuarioService);
  private toaster = inject(ToasterService);
  private router = inject(Router);

  passwordForm: FormGroup;
  isLoading = false;
  showCurrentPassword = false;
  showNewPassword = false;
  showConfirmPassword = false;

  constructor() {
    this.passwordForm = this.fb.group({
      passwordActual: ['', [Validators.required, Validators.minLength(6)]],
      nuevoPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmarNuevoPassword: ['', [Validators.required]]
    }, {
      validators: this.passwordMatchValidator
    });
  }

  passwordMatchValidator(form: FormGroup) {
    const newPassword = form.get('nuevoPassword');
    const confirmPassword = form.get('confirmarNuevoPassword');

    if (newPassword && confirmPassword) {
      return newPassword.value === confirmPassword.value ? null : { passwordMismatch: true };
    }
    return null;
  }

  togglePasswordVisibility(field: 'current' | 'new' | 'confirm'): void {
    switch (field) {
      case 'current':
        this.showCurrentPassword = !this.showCurrentPassword;
        break;
      case 'new':
        this.showNewPassword = !this.showNewPassword;
        break;
      case 'confirm':
        this.showConfirmPassword = !this.showConfirmPassword;
        break;
    }
  }

  onSubmit(): void {
    if (this.passwordForm.invalid) {
      Object.keys(this.passwordForm.controls).forEach(key => {
        this.passwordForm.get(key)?.markAsTouched();
      });
      this.toaster.warn('::FormularioInvalido', '::Advertencia');
      return;
    }

    this.isLoading = true;

    const input: CambiarPasswordDto = {
      passwordActual: this.passwordForm.value.passwordActual,
      nuevoPassword: this.passwordForm.value.nuevoPassword,
      confirmarNuevoPassword: this.passwordForm.value.confirmarNuevoPassword
    };

    this.usuarioService.cambiarPassword(input)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: () => {
          this.toaster.success('::PasswordChangedSuccessfully', '::Success');
          this.passwordForm.reset();
          // Redirigir al perfil después de 2 segundos
          setTimeout(() => {
            this.router.navigate(['/account/manage']);
          }, 2000);
        },
        error: (error) => {
          console.error('Error al cambiar contraseña:', error);
          if (error.error?.error?.message) {
            this.toaster.error(error.error.error.message, '::Error');
          } else {
            this.toaster.error('::ErrorChangingPassword', '::Error');
          }
        }
      });
  }

  cancel(): void {
    this.router.navigate(['/account/manage']);
  }
}
