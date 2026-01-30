import { Component, OnInit, inject, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToasterService } from '@abp/ng.theme.shared';
import { LocalizationPipe } from '@abp/ng.core';
import { UsuarioService } from '../../proxy/usuarios/usuario.service';
import { ActualizarPerfilDto, ActualizarPreferenciasDto, FrecuenciaNotificacion, UsuarioDto } from '../../proxy/usuarios/models';
import { finalize } from 'rxjs/operators';
import { ConfirmationService } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-mi-cuenta',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule, LocalizationPipe],
  templateUrl: './mi-cuenta.component.html',
  styleUrl: './mi-cuenta.component.scss'
})
export class MiCuentaComponent implements OnInit {
  private fb = inject(FormBuilder);
  private usuarioService = inject(UsuarioService);
  private toaster = inject(ToasterService);
  private router = inject(Router);
  private confirmation = inject(ConfirmationService);

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  perfilForm!: FormGroup;
  preferenciasForm!: FormGroup;

  usuario: UsuarioDto | null = null;
  isLoadingPerfil = false;
  isLoadingPreferencias = false;

  fotoPreview: string | null = null;
  selectedFile: File | null = null;

  // Control de tabs
  activeTab: 'perfil' | 'preferencias' = 'perfil';

  // Enums para el template
  FrecuenciaNotificacion = FrecuenciaNotificacion;

  ngOnInit(): void {
    this.initForms();
    this.cargarPerfil();
  }

  initForms(): void {
    this.perfilForm = this.fb.group({
      nombreCompleto: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email]],
      fotoPerfilUrl: ['']
    });

    this.preferenciasForm = this.fb.group({
      recibirEnPantalla: [true],
      recibirPorEmail: [false],
      frecuencia: [FrecuenciaNotificacion.Inmediata]
    });
  }

  cargarPerfil(): void {
    this.isLoadingPerfil = true;
    this.usuarioService.obtenerPerfilActual()
      .pipe(finalize(() => this.isLoadingPerfil = false))
      .subscribe({
        next: (usuario) => {
          this.usuario = usuario;
          this.perfilForm.patchValue({
            nombreCompleto: usuario.nombreCompleto,
            email: usuario.email,
            fotoPerfilUrl: usuario.fotoPerfilUrl || ''
          });
          this.preferenciasForm.patchValue({
            recibirEnPantalla: usuario.preferencias.recibirEnPantalla,
            recibirPorEmail: usuario.preferencias.recibirPorEmail,
            frecuencia: usuario.preferencias.frecuencia
          });
          this.fotoPreview = usuario.fotoPerfilUrl || null;
        },
        error: (error) => {
          this.toaster.error('::ErrorAlCargarPerfil', '::Error');
          console.error('Error al cargar perfil:', error);
        }
      });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];

      // Validar tipo de archivo
      if (!file.type.startsWith('image/')) {
        this.toaster.error('::SoloImagenes', '::Error');
        return;
      }

      // Validar tamaño (máx 5MB)
      if (file.size > 5 * 1024 * 1024) {
        this.toaster.error('::ImagenMuyGrande', '::Error');
        return;
      }

      this.selectedFile = file;

      // Preview de la imagen
      const reader = new FileReader();
      reader.onload = (e) => {
        this.fotoPreview = e.target?.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  guardarPerfil(): void {
    if (this.perfilForm.invalid) {
      this.toaster.warn('::FormularioInvalido', '::Advertencia');
      return;
    }

    this.isLoadingPerfil = true;

    // Si hay una imagen seleccionada, primero la subimos
    if (this.selectedFile) {
      this.subirImagen().then((url) => {
        this.actualizarPerfil(url);
      }).catch((error) => {
        this.isLoadingPerfil = false;
        this.toaster.error('::ErrorAlSubirImagen', '::Error');
        console.error('Error al subir imagen:', error);
      });
    } else {
      this.actualizarPerfil(this.perfilForm.value.fotoPerfilUrl);
    }
  }

  private async subirImagen(): Promise<string> {
    if (!this.selectedFile) {
      throw new Error('No hay archivo seleccionado');
    }

    // Por ahora, usar la preview como URL (en producción deberías subir a un servidor)
    // TODO: Implementar subida real al servidor
    return this.fotoPreview || '';
  }

  private actualizarPerfil(fotoUrl: string): void {
    const input: ActualizarPerfilDto = {
      nombreCompleto: this.perfilForm.value.nombreCompleto,
      email: this.perfilForm.value.email,
      fotoPerfilUrl: fotoUrl || undefined
    };

    this.usuarioService.actualizarPerfil(input)
      .pipe(finalize(() => this.isLoadingPerfil = false))
      .subscribe({
        next: () => {
          this.toaster.success('::PerfilActualizadoExitosamente', '::Exito');
          this.cargarPerfil();
        },
        error: (error) => {
          this.toaster.error('::ErrorAlActualizarPerfil', '::Error');
          console.error('Error al actualizar perfil:', error);
        }
      });
  }

  guardarPreferencias(): void {
    if (this.preferenciasForm.invalid) {
      this.toaster.warn('::FormularioInvalido', '::Advertencia');
      return;
    }

    this.isLoadingPreferencias = true;

    const input: ActualizarPreferenciasDto = {
      recibirEnPantalla: this.preferenciasForm.value.recibirEnPantalla,
      recibirPorEmail: this.preferenciasForm.value.recibirPorEmail,
      frecuencia: this.preferenciasForm.value.frecuencia
    };

    this.usuarioService.actualizarPreferencias(input)
      .pipe(finalize(() => this.isLoadingPreferencias = false))
      .subscribe({
        next: () => {
          this.toaster.success('::PreferenciasActualizadasExitosamente', '::Exito');
        },
        error: (error) => {
          this.toaster.error('::ErrorAlActualizarPreferencias', '::Error');
          console.error('Error al actualizar preferencias:', error);
        }
      });
  }

  eliminarFoto(): void {
    // Mostrar confirmación antes de eliminar
    this.confirmation
      .warn('::ConfirmDeleteProfilePicture', '::AreYouSure', {
        yesText: '::Yes',
      })
      .subscribe((status) => {
        if (status === 'confirm') {
          this.fotoPreview = null;
          this.selectedFile = null;
          this.perfilForm.patchValue({ fotoPerfilUrl: '' });

          // Limpiar el input de archivo
          if (this.fileInput && this.fileInput.nativeElement) {
            this.fileInput.nativeElement.value = '';
          }

          // Si el usuario tenía una foto guardada, actualizamos el perfil inmediatamente
          if (this.usuario?.fotoPerfilUrl) {
            this.actualizarPerfil('');
          }
        }
      });
  }

  cambiarTab(tab: 'perfil' | 'preferencias'): void {
    this.activeTab = tab;
  }
}
