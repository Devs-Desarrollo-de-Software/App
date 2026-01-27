import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ToasterService } from '@abp/ng.theme.shared';
import { UsuarioService } from '../../proxy/usuarios/usuario.service';
import { UsuarioDto, TipoRol, ActualizarUsuarioDto } from '../../proxy/usuarios/models';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-editar-usuario',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './editar-usuario.component.html',
  styleUrls: ['./editar-usuario.component.scss'],
})
export class EditarUsuarioComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly toaster = inject(ToasterService);
  private readonly usuarioService = inject(UsuarioService);

  usuarioForm!: FormGroup;
  loading = false;
  usuarioId!: string;
  TipoRol = TipoRol;

  ngOnInit(): void {
    this.inicializarFormulario();
    this.usuarioId = this.route.snapshot.paramMap.get('id')!;
    this.cargarUsuario();
  }

  inicializarFormulario(): void {
    this.usuarioForm = this.fb.group({
      nombreCompleto: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email]],
      fotoPerfilUrl: [''],
      rol: [TipoRol.Usuario, Validators.required],
      estaActivo: [true],
    });
  }

  cargarUsuario(): void {
    this.loading = true;
    this.usuarioService
      .obtenerUsuarioPorId(this.usuarioId)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (usuario: UsuarioDto) => {
          this.usuarioForm.patchValue({
            nombreCompleto: usuario.nombreCompleto,
            email: usuario.email,
            fotoPerfilUrl: usuario.fotoPerfilUrl || '',
            rol: usuario.rol,
            estaActivo: usuario.estaActivo,
          });
        },
        error: () => {
          this.toaster.error('::ErrorAlCargarUsuario', '::Error');
          this.volver();
        },
      });
  }

  guardar(): void {
    if (this.usuarioForm.invalid) {
      this.toaster.warn('::FormularioInvalido', '::Advertencia');
      return;
    }

    this.loading = true;
    const input: ActualizarUsuarioDto = this.usuarioForm.value;

    this.usuarioService
      .actualizarUsuario(this.usuarioId, input)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: () => {
          this.toaster.success('::UsuarioActualizadoExitosamente', '::Exito');
          this.volver();
        },
        error: () => {
          this.toaster.error('::ErrorAlActualizarUsuario', '::Error');
        },
      });
  }

  volver(): void {
    this.router.navigate(['/usuarios/lista']);
  }
}
