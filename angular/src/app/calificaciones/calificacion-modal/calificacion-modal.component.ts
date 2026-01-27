import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '@abp/ng.core';
import { CalificacionFormComponent } from '../calificacion-form/calificacion-form.component';

// Modal para calificar un destino turístico
// Contiene el formulario de calificación y maneja su visualización
@Component({
  selector: 'app-calificacion-modal',
  standalone: true,
  imports: [
    CommonModule,
    CoreModule,
    CalificacionFormComponent,
  ],
  templateUrl: './calificacion-modal.component.html',
  styleUrls: ['./calificacion-modal.component.scss'],
})
export class CalificacionModalComponent implements OnInit {
  // ID del destino a calificar
  @Input() destinoId!: string;

  // Nombre del destino (para mostrar en el modal)
  @Input() destinoNombre: string = '';

  // Controla la visibilidad del modal
  @Input() isVisible: boolean = false;

  // Evento emitido cuando se cierra el modal
  @Output() close = new EventEmitter<void>();

  // Controla la visibilidad del formulario dentro del modal
  mostrarFormulario: boolean = true;

  ngOnInit(): void {
    // Componente inicializado
  }

  cerrarModal(): void {
    this.close.emit();
  }

  onCalificacionCreada(): void {
    // Ocultar formulario después de calificar
    this.mostrarFormulario = false;
    // Recargar promedio y comentarios (los componentes lo harán automáticamente con OnInit)
  }

  mostrarFormularioNuevamente(): void {
    this.mostrarFormulario = true;
  }

  onBackdropClick(event: MouseEvent): void {
    // Cerrar solo si se hace clic en el backdrop, no en el contenido del modal
    if (event.target === event.currentTarget) {
      this.cerrarModal();
    }
  }
}
