import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '@abp/ng.core';
import { CalificacionFormComponent } from '../calificacion-form/calificacion-form.component';
import { CalificacionPromedioComponent } from '../calificacion-promedio/calificacion-promedio.component';
import { ComentariosListComponent } from '../comentarios-list/comentarios-list.component';

@Component({
  selector: 'app-calificacion-modal',
  standalone: true,
  imports: [
    CommonModule,
    CoreModule,
    CalificacionFormComponent,
    CalificacionPromedioComponent,
    ComentariosListComponent,
  ],
  templateUrl: './calificacion-modal.component.html',
  styleUrls: ['./calificacion-modal.component.scss'],
})
export class CalificacionModalComponent implements OnInit {
  @Input() destinoId!: string;
  @Input() destinoNombre: string = '';
  @Input() isVisible: boolean = false;
  @Output() close = new EventEmitter<void>();

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
