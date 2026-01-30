import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { CalificacionService, CalificacionDto, UpdateCalificacionDto } from 'src/app/proxy/calificaciones';
import { PagedAndSortedResultRequestDto } from '@abp/ng.core';

interface CalificacionConEdicion extends CalificacionDto {
  editando?: boolean;
  puntuacionEdit?: number;
  comentarioEdit?: string;
  puntuacionHover?: number;
}

@Component({
  selector: 'app-mis-calificaciones-list',
  standalone: true,
  imports: [CommonModule, FormsModule, CoreModule],
  templateUrl: './mis-calificaciones-list.component.html',
  styleUrls: ['./mis-calificaciones-list.component.scss'],
})
export class MisCalificacionesListComponent implements OnInit {
  private readonly calificacionService = inject(CalificacionService);
  private readonly toaster = inject(ToasterService);
  private readonly confirmation = inject(ConfirmationService);

  calificaciones: CalificacionConEdicion[] = [];
  loading: boolean = false;
  error: boolean = false;

  estrellas = [1, 2, 3, 4, 5];

  ngOnInit(): void {
    this.cargarCalificaciones();
  }

  cargarCalificaciones(): void {
    this.loading = true;
    this.error = false;

    const input: PagedAndSortedResultRequestDto = {
      skipCount: 0,
      maxResultCount: 100,
      sorting: 'creationTime DESC',
    };

    this.calificacionService.getList(input).subscribe({
      next: (data) => {
        this.calificaciones = data.items || [];
        this.loading = false;
      },
      error: (err) => {
        console.error('Error al cargar calificaciones:', err);
        this.error = true;
        this.loading = false;
        this.toaster.error('Error al cargar tus calificaciones.');
      },
    });
  }

  iniciarEdicion(calificacion: CalificacionConEdicion): void {
    calificacion.editando = true;
    calificacion.puntuacionEdit = calificacion.puntuacion;
    calificacion.comentarioEdit = calificacion.comentario || '';
    calificacion.puntuacionHover = 0;
  }

  cancelarEdicion(calificacion: CalificacionConEdicion): void {
    calificacion.editando = false;
    calificacion.puntuacionEdit = undefined;
    calificacion.comentarioEdit = undefined;
    calificacion.puntuacionHover = 0;
  }

  seleccionarPuntuacion(calificacion: CalificacionConEdicion, puntos: number): void {
    if (calificacion.editando) {
      calificacion.puntuacionEdit = puntos;
    }
  }

  hoverEstrella(calificacion: CalificacionConEdicion, puntos: number): void {
    if (calificacion.editando) {
      calificacion.puntuacionHover = puntos;
    }
  }

  resetHover(calificacion: CalificacionConEdicion): void {
    if (calificacion.editando) {
      calificacion.puntuacionHover = 0;
    }
  }

  guardarEdicion(calificacion: CalificacionConEdicion): void {
    if (!calificacion.id || !calificacion.puntuacionEdit) {
      this.toaster.warn('Debes seleccionar una puntuación.');
      return;
    }

    const input: UpdateCalificacionDto = {
      puntuacion: calificacion.puntuacionEdit,
      comentario: calificacion.comentarioEdit?.trim() || undefined,
    };

    this.calificacionService.update(calificacion.id, input).subscribe({
      next: (updated) => {
        this.toaster.success('Calificación actualizada correctamente.');
        calificacion.puntuacion = updated.puntuacion;
        calificacion.comentario = updated.comentario;
        this.cancelarEdicion(calificacion);
      },
      error: (err) => {
        console.error(err);
        this.toaster.error('Error al actualizar la calificación.');
      },
    });
  }

  eliminarCalificacion(calificacion: CalificacionConEdicion): void {
    if (!calificacion.id) {
      return;
    }

    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.calificacionService.delete(calificacion.id!).subscribe({
          next: () => {
            this.toaster.success('Calificación eliminada correctamente.');
            this.calificaciones = this.calificaciones.filter((c) => c.id !== calificacion.id);
          },
          error: (err) => {
            console.error(err);
            this.toaster.error('Error al eliminar la calificación.');
          },
        });
      }
    });
  }

  obtenerClaseEstrella(calificacion: CalificacionConEdicion, indice: number): string {
    if (calificacion.editando) {
      const puntosActivos = calificacion.puntuacionHover || calificacion.puntuacionEdit || 0;
      return indice <= puntosActivos ? 'star-filled' : 'star-empty';
    } else {
      return indice <= calificacion.puntuacion ? 'star-filled' : 'star-empty';
    }
  }

  formatearFecha(fecha: string | undefined): string {
    if (!fecha) return '';
    const date = new Date(fecha);
    return date.toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  }
}
