import { Component, inject, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '@abp/ng.core';
import { CalificacionService, ComentarioDto, ListarComentariosDto } from 'src/app/proxy/calificaciones';

@Component({
  selector: 'app-comentarios-list',
  standalone: true,
  imports: [CommonModule, CoreModule],
  templateUrl: './comentarios-list.component.html',
  styleUrls: ['./comentarios-list.component.scss'],
})
export class ComentariosListComponent implements OnInit {
  @Input() destinoId!: string;

  private readonly calificacionService = inject(CalificacionService);

  comentarios: ComentarioDto[] = [];
  loading: boolean = false;
  error: boolean = false;

  estrellas = [1, 2, 3, 4, 5];

  ngOnInit(): void {
    if (this.destinoId) {
      this.cargarComentarios();
    }
  }

  cargarComentarios(): void {
    if (!this.destinoId) {
      return;
    }

    this.loading = true;
    this.error = false;

    this.calificacionService.getListComentarios(this.destinoId).subscribe({
      next: (data: ListarComentariosDto) => {
        this.comentarios = data.comentarios || [];
        this.loading = false;
      },
      error: (err) => {
        console.error('Error al cargar comentarios:', err);
        // No mostrar error si simplemente no hay comentarios aún
        this.comentarios = [];
        this.loading = false;
      },
    });
  }

  obtenerClaseEstrella(comentario: ComentarioDto, indice: number): string {
    return indice <= comentario.puntuacion ? 'star-filled' : 'star-empty';
  }

  formatearFecha(fecha: string): string {
    const date = new Date(fecha);
    const ahora = new Date();
    const diferenciaDias = Math.floor(
      (ahora.getTime() - date.getTime()) / (1000 * 60 * 60 * 24)
    );

    if (diferenciaDias === 0) {
      return 'Hoy';
    } else if (diferenciaDias === 1) {
      return 'Ayer';
    } else if (diferenciaDias < 7) {
      return `Hace ${diferenciaDias} días`;
    } else if (diferenciaDias < 30) {
      const semanas = Math.floor(diferenciaDias / 7);
      return `Hace ${semanas} ${semanas === 1 ? 'semana' : 'semanas'}`;
    } else {
      return date.toLocaleDateString('es-ES', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
      });
    }
  }
}
