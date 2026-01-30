import { Component, Input, OnInit, OnChanges, SimpleChanges, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '@abp/ng.core';
import { CalificacionService, ListarComentariosDto, ComentarioDto } from 'src/app/proxy/calificaciones';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-comentarios-list',
  standalone: true,
  imports: [CommonModule, CoreModule],
  templateUrl: './comentarios-list.component.html',
  styleUrls: ['./comentarios-list.component.scss']
})
export class ComentariosListComponent implements OnInit, OnChanges {
  @Input() destinoId: string = '';

  private readonly calificacionService = inject(CalificacionService);

  comentarios: ComentarioDto[] = [];
  loading = false;
  errorMessage: string | null = null;

  ngOnInit(): void {
    if (this.destinoId) {
      this.cargarComentarios();
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['destinoId'] && !changes['destinoId'].firstChange) {
      this.cargarComentarios();
    }
  }

  private cargarComentarios(): void {
    if (!this.destinoId) {
      return;
    }

    this.loading = true;
    this.errorMessage = null;

    this.calificacionService.getListComentarios(this.destinoId)
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (result: ListarComentariosDto) => {
          this.comentarios = result.comentarios || [];
        },
        error: (err) => {
          console.error('Error al cargar comentarios:', err);
          this.errorMessage = 'Error al cargar los comentarios';
          this.comentarios = [];
        }
      });
  }

  /**
   * Obtiene el array de estrellas para mostrar la puntuación
   */
  getEstrellas(puntuacion: number): string[] {
    const estrellas: string[] = [];
    for (let i = 1; i <= 5; i++) {
      estrellas.push(i <= puntuacion ? 'full' : 'empty');
    }
    return estrellas;
  }

  /**
   * Formatea la fecha de creación
   */
  formatearFecha(fecha: string | undefined): string {
    if (!fecha) return '';
    const date = new Date(fecha);
    return date.toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
