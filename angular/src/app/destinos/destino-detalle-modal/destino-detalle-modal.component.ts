import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '@abp/ng.core';
import { DestinoService, CityDetailDto } from 'src/app/proxy/destinos';

@Component({
  selector: 'app-destino-detalle-modal',
  standalone: true,
  imports: [CommonModule, CoreModule],
  templateUrl: './destino-detalle-modal.component.html',
  styleUrls: ['./destino-detalle-modal.component.scss'],
})
export class DestinoDetalleModalComponent implements OnInit {
  @Input() cityId!: number;
  @Input() cityName: string = '';
  @Input() isVisible: boolean = false;
  @Output() close = new EventEmitter<void>();

  private readonly destinoService = inject(DestinoService);

  detalles: CityDetailDto | null = null;
  loading: boolean = false;
  error: boolean = false;

  ngOnInit(): void {
    if (this.isVisible && this.cityId) {
      this.cargarDetalles();
    }
  }

  ngOnChanges(): void {
    if (this.isVisible && this.cityId && !this.detalles) {
      this.cargarDetalles();
    }
  }

  cargarDetalles(): void {
    this.loading = true;
    this.error = false;

    this.destinoService.obtenerDetalleCiudad(this.cityId).subscribe({
      next: (data) => {
        this.detalles = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error al cargar detalles:', err);
        this.error = true;
        this.loading = false;
      },
    });
  }

  cerrarModal(): void {
    this.detalles = null;
    this.close.emit();
  }

  verEnMapa(): void {
    if (this.detalles) {
      const url = `https://www.google.com/maps/search/?api=1&query=${this.detalles.latitude},${this.detalles.longitude}`;
      window.open(url, '_blank');
    }
  }

  onBackdropClick(event: MouseEvent): void {
    if (event.target === event.currentTarget) {
      this.cerrarModal();
    }
  }

  formatearNumero(num: number | undefined): string {
    if (!num) return 'N/A';
    return num.toLocaleString('es-ES');
  }
}
