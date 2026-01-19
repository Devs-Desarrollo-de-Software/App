import { Component, inject, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '@abp/ng.core';
import { CalificacionService, PromedioCalificacionDto } from 'src/app/proxy/calificaciones';

@Component({
  selector: 'app-calificacion-promedio',
  standalone: true,
  imports: [CommonModule, CoreModule],
  templateUrl: './calificacion-promedio.component.html',
  styleUrls: ['./calificacion-promedio.component.scss'],
})
export class CalificacionPromedioComponent implements OnInit {
  @Input() destinoId!: string;
  @Input() showLabel: boolean = true;
  @Input() size: 'small' | 'medium' | 'large' = 'medium';

  private readonly calificacionService = inject(CalificacionService);

  promedio: number = 0;
  totalCalificaciones: number = 0;
  loading: boolean = false;
  error: boolean = false;

  // Array para renderizar las estrellas (0.5, 1, 1.5, 2, etc.)
  estrellas = [1, 2, 3, 4, 5];

  ngOnInit(): void {
    if (this.destinoId) {
      this.cargarPromedio();
    }
  }

  cargarPromedio(): void {
    if (!this.destinoId) {
      return;
    }

    this.loading = true;
    this.error = false;

    this.calificacionService.getPromedio(this.destinoId).subscribe({
      next: (data: PromedioCalificacionDto) => {
        this.promedio = data.promedioCalificacion || 0;
        this.totalCalificaciones = data.totalCalificaciones || 0;
        this.loading = false;
      },
      error: err => {
        console.error('Error al cargar promedio:', err);
        // No mostrar error si simplemente no hay calificaciones aún
        this.promedio = 0;
        this.totalCalificaciones = 0;
        this.loading = false;
      },
    });
  }

  obtenerClaseEstrella(indice: number): string {
    const diferencia = this.promedio - (indice - 1);

    if (diferencia >= 1) {
      return 'star-filled';
    } else if (diferencia >= 0.5) {
      return 'star-half';
    } else {
      return 'star-empty';
    }
  }

  get sizeClass(): string {
    return `size-${this.size}`;
  }
}
