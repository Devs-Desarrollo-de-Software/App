import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { CalificacionService, CreateCalificacionDto } from 'src/app/proxy/calificaciones';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-calificacion-form',
  standalone: true,
  imports: [CommonModule, FormsModule, CoreModule],
  templateUrl: './calificacion-form.component.html',
  styleUrls: ['./calificacion-form.component.scss'],
})
export class CalificacionFormComponent {
  @Input() destinoId!: string;
  @Input() destinoNombre: string = '';
  @Output() calificacionCreada = new EventEmitter<void>();

  private readonly calificacionService = inject(CalificacionService);
  private readonly toaster = inject(ToasterService);

  puntuacion: number = 0;
  puntuacionHover: number = 0;
  comentario: string = '';
  loading: boolean = false;

  // Array para renderizar las estrellas
  estrellas = [1, 2, 3, 4, 5];

  seleccionarPuntuacion(puntos: number): void {
    this.puntuacion = puntos;
  }

  hoverEstrella(puntos: number): void {
    this.puntuacionHover = puntos;
  }

  resetHover(): void {
    this.puntuacionHover = 0;
  }

  obtenerClaseEstrella(indice: number): string {
    const puntosActivos = this.puntuacionHover || this.puntuacion;
    return indice <= puntosActivos ? 'star-filled' : 'star-empty';
  }

  enviarCalificacion(): void {
    if (!this.destinoId) {
      this.toaster.error('No se especificó el destino');
      return;
    }

    if (this.puntuacion === 0) {
      this.toaster.warn('Debes seleccionar una puntuación');
      return;
    }

    this.loading = true;

    const input: CreateCalificacionDto = {
      destinoId: this.destinoId,
      puntuacion: this.puntuacion,
      comentario: this.comentario.trim() || undefined,
    };

    this.calificacionService
      .create(input)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: () => {
          this.toaster.success('¡Calificación enviada exitosamente!');
          this.limpiarFormulario();
          this.calificacionCreada.emit();
        },
        error: (err) => {
          if (err.error?.error?.message?.includes('Ya has calificado')) {
            this.toaster.warn('Ya has calificado este destino anteriormente');
          } else {
            this.toaster.error('Error al enviar la calificación');
          }
          console.error(err);
        },
      });
  }

  limpiarFormulario(): void {
    this.puntuacion = 0;
    this.puntuacionHover = 0;
    this.comentario = '';
  }

  cancelar(): void {
    this.limpiarFormulario();
  }
}
