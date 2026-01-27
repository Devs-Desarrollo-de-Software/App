import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { ToasterService, Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { DestinoService, CityDto, DestinoDto } from 'src/app/proxy/destinos';
import { finalize } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { CalificacionModalComponent } from 'src/app/calificaciones/calificacion-modal/calificacion-modal.component';
import { DestinoDetalleModalComponent } from '../destino-detalle-modal/destino-detalle-modal.component';
import { CalificacionService, PromedioCalificacionDto } from 'src/app/proxy/calificaciones';

// Componente principal de búsqueda y gestión de destinos
// Permite buscar ciudades desde la API externa GeoDB, guardarlas como destinos
// y ver calificaciones promedio de cada destino
@Component({
  selector: 'app-destinos-list',
  standalone: true,
  imports: [CommonModule, FormsModule, CoreModule, CalificacionModalComponent, DestinoDetalleModalComponent],
  templateUrl: './destinos-list.html',
  styleUrls: ['./destinos-list.scss'],
})
export class DestinosList implements OnInit, OnDestroy {
  // Servicios inyectados
  private readonly destinoService = inject(DestinoService);
  private readonly calificacionService = inject(CalificacionService);
  private readonly toaster = inject(ToasterService);
  private readonly confirmation = inject(ConfirmationService);

  /**
   * Texto que escribe el usuario para buscar por nombre
   * (Nota: El backend usa 'paisPrefix' y 'regionPrefix', aquí usamos nombreCiudad para filtrar por nombre si está presente
   * o si se quiere usar el endpoint de buscar por nombre. Pero para cumplir requerimientos usaremos filtrarCiudades)
   */
  nombrePais = '';
  minPoblacion: number | null = null;
  nombreRegion = '';
  nombreCiudad = ''; // Restore nombreCiudad

  /**
   * Resultado de la búsqueda
   */
  ciudades: CityDto[] = [];

  /**
   * Indicadores de estado
   */
  loading = false;
  errorMessage: string | null = null;

  /**
   * Mapa para trackear IDs de destinos guardados (city.id -> destino.id)
   */
  destinosGuardadosMap = new Map<number, string>();

  /**
   * Mapa para guardar promedios de calificaciones (destino.id -> PromedioCalificacionDto)
   */
  promediosMap = new Map<string, PromedioCalificacionDto>();

  /**
   * Set de destinos que el usuario ya ha calificado (destino.id)
   */
  destinosCalificadosSet = new Set<string>();

  /**
   * Control del modal de calificaciones
   */
  modalVisible = false;
  modalDestinoId: string = '';
  modalDestinoNombre: string = '';

  /**
   * Control del modal de detalles
   */
  detalleModalVisible = false;
  detalleCityId: number = 0;
  detalleCityName: string = '';
  detalleDestinoId: string = '';

  /**
   * Subject para implementar debounce en la búsqueda
   */
  private searchSubject = new Subject<void>();

  ngOnInit(): void {
    // Cargar destinos guardados del usuario
    this.cargarDestinosGuardados();

    // Cargar calificaciones del usuario para saber qué destinos ya calificó
    this.cargarCalificacionesUsuario();

    // Cargar destinos populares al inicio
    this.loadPopularDestinations();

    // Configurar debounce para búsqueda automática (opcional)
    this.searchSubject
      .pipe(
        debounceTime(500), // Espera 500ms después de que el usuario deje de escribir
        distinctUntilChanged()
      )
      .subscribe(() => {
        this.performSearch();
      });
  }

  /**
   * Carga los destinos guardados del usuario para mapear city.id -> destino.id
   */
  private cargarDestinosGuardados(): void {
    const input = {
      skipCount: 0,
      maxResultCount: 1000,
    };

    this.destinoService.getList(input).subscribe({
      next: (result) => {
        // Mapear cada destino guardado: apiCityId -> destinoId
        result.items.forEach((destino: any) => {
          if (destino.apiCityId && destino.id) {
            this.destinosGuardadosMap.set(destino.apiCityId, destino.id);
            // Cargar el promedio de calificaciones para este destino
            this.cargarPromedioCalificacion(destino.id);
          }
        });
      },
      error: (err) => {
        console.error('Error al cargar destinos guardados:', err);
      },
    });
  }

  /**
   * Carga las calificaciones del usuario para saber qué destinos ya calificó
   */
  private cargarCalificacionesUsuario(): void {
    const input = {
      skipCount: 0,
      maxResultCount: 1000,
    };

    this.calificacionService.getList(input).subscribe({
      next: (result) => {
        // Guardar los IDs de destinos que el usuario ya calificó
        result.items.forEach((calificacion) => {
          if (calificacion.destinoId) {
            this.destinosCalificadosSet.add(calificacion.destinoId);
          }
        });
      },
      error: (err) => {
        console.error('Error al cargar calificaciones del usuario:', err);
      }
    });
  }

  /**
   * Carga el promedio de calificaciones para un destino específico
   */
  private cargarPromedioCalificacion(destinoId: string): void {
    this.calificacionService.getPromedio(destinoId).subscribe({
      next: (promedio) => {
        this.promediosMap.set(destinoId, promedio);
      },
      error: (err) => {
        // Cualquier error (404, 400, 500, etc.) se trata como "sin calificaciones"
        // Esto evita que se muestren errores al usuario
        this.promediosMap.set(destinoId, {
          destinoId: destinoId,
          promedioCalificacion: 0,
          totalCalificaciones: 0
        });

        // Solo logueamos en consola si no es un 404 (que es esperado)
        if (err.status !== 404) {
          console.warn('Error al cargar promedio de calificaciones:', err);
        }
      }
    });
  }

  ngOnDestroy(): void {
    // Importante: desuscribirse para evitar memory leaks
    this.searchSubject.complete();
  }

  /**
   * Método que se llama cuando el usuario escribe en los inputs
   * (opcional, para búsqueda en tiempo real con debounce)
   */
  onSearchInput(): void {
    this.searchSubject.next();
  }

  /**
   * Llama a la API para buscar ciudades
   * Ahora busca por nombre O por país, o ambos
   */
  onSearch(): void {
    this.performSearch();
  }

  /**
   * Ejecuta la búsqueda real
   */
  /**
   * Ejecuta la búsqueda real usando los filtros avanzados
   */
  private performSearch(): void {
    const termPais = this.nombrePais.trim();
    const termRegion = this.nombreRegion.trim();
    // Asegurar que la población mínima no sea negativa
    let minPop = this.minPoblacion || 0;
    if (minPop < 0) {
      minPop = 0;
      this.minPoblacion = 0; // Actualizar el modelo para reflejar el cambio
    }
    const termCiudad = this.nombreCiudad ? this.nombreCiudad.trim() : '';

    // Si todo esta vacio, cargar populares
    if (!termPais && !termRegion && minPop <= 0 && !termCiudad) {
       this.loadPopularDestinations();
       return;
    }

    this.loading = true;
    this.errorMessage = null;

    this.destinoService.filtrarCiudades(termPais, minPop, termRegion, termCiudad)
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (result) => {
           this.ciudades = result;

           if (this.ciudades.length === 0) {
              this.errorMessage = 'No se encontraron ciudades con esos criterios.';
           }
        },
        error: (err) => {
           console.error(err);
           this.errorMessage = 'Ocurrió un error al buscar.';
        }
      });
  }

  guardarDestino(ciudad: CityDto): void {
      if (!ciudad.id) {
          this.toaster.error('No se pudo obtener el ID de la ciudad');
          return;
      }

      this.destinoService.guardarDestinoDesdeApi(ciudad.id).subscribe({
          next: (destino) => {
              this.toaster.success(`Destino ${destino.nombre} guardado correctamente!`);
              // Guardar el ID del destino para poder mostrar calificaciones
              if (destino.id && ciudad.id) {
                  this.destinosGuardadosMap.set(ciudad.id, destino.id);
                  // Cargar el promedio de calificaciones para el nuevo destino
                  this.cargarPromedioCalificacion(destino.id);
              }
          },
          error: (err: any) => {
              if (err.error && err.error.code === 'DestinoYaExiste') {
                  this.toaster.warn('Este destino ya está guardado en tu lista.');
              } else {
                  this.toaster.error('Error al guardar el destino.');
              }
          }
      });
  }

  /**
   * Obtiene el ID del destino guardado para una ciudad
   */
  getDestinoId(ciudad: CityDto): string | undefined {
      return ciudad.id ? this.destinosGuardadosMap.get(ciudad.id) : undefined;
  }

  /**
   * Abre el modal de calificaciones para un destino
   */
  abrirModalCalificaciones(ciudad: CityDto): void {
      const destinoId = this.getDestinoId(ciudad);
      if (destinoId) {
          this.modalDestinoId = destinoId;
          this.modalDestinoNombre = ciudad.name || '';
          this.modalVisible = true;
      } else {
          this.toaster.warn('Primero debes guardar este destino para poder calificarlo.');
      }
  }

  /**
   * Cierra el modal de calificaciones
   */
  cerrarModalCalificaciones(): void {
      // Guardar el ID antes de limpiarlo para recargar el promedio
      const destinoId = this.modalDestinoId;

      this.modalVisible = false;
      this.modalDestinoId = '';
      this.modalDestinoNombre = '';

      // Recargar el promedio después de cerrar el modal (por si hubo cambios)
      if (destinoId) {
          this.cargarPromedioCalificacion(destinoId);
          // Marcar este destino como calificado (el usuario acaba de calificarlo)
          this.destinosCalificadosSet.add(destinoId);
      }
  }

  /**
   * Obtiene el promedio de calificaciones para un destino
   */
  getPromedio(ciudad: CityDto): PromedioCalificacionDto | undefined {
      const destinoId = this.getDestinoId(ciudad);
      return destinoId ? this.promediosMap.get(destinoId) : undefined;
  }

  /**
   * Obtiene el array de estrellas para mostrar el promedio
   * Retorna un array con 5 elementos: 'full', 'half', o 'empty'
   */
  getEstrellas(promedio: number): string[] {
      const estrellas: string[] = [];
      const promedioRedondeado = Math.round(promedio * 2) / 2; // Redondear a 0.5

      for (let i = 1; i <= 5; i++) {
          if (i <= promedioRedondeado) {
              estrellas.push('full');
          } else if (i - 0.5 === promedioRedondeado) {
              estrellas.push('half');
          } else {
              estrellas.push('empty');
          }
      }

      return estrellas;
  }

  /**
   * Verifica si el usuario ya calificó este destino
   */
  yaCalificado(ciudad: CityDto): boolean {
      const destinoId = this.getDestinoId(ciudad);
      return destinoId ? this.destinosCalificadosSet.has(destinoId) : false;
  }

  /**
   * Elimina un destino guardado
   */
  eliminarDestino(ciudad: CityDto): void {
      const destinoId = this.getDestinoId(ciudad);
      if (!destinoId) {
          this.toaster.warn('Este destino no está guardado.');
          return;
      }

      this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
          if (status === Confirmation.Status.confirm) {
              this.destinoService.delete(destinoId).subscribe({
                  next: () => {
                      this.toaster.success(`Destino ${ciudad.name} eliminado correctamente.`);
                      // Remover del mapa
                      if (ciudad.id) {
                          this.destinosGuardadosMap.delete(ciudad.id);
                      }
                  },
                  error: (err) => {
                      console.error(err);
                      this.toaster.error('Error al eliminar el destino.');
                  }
              });
          }
      });
  }

  /**
   * Abre el modal de detalles de una ciudad
   */
  abrirModalDetalles(ciudad: CityDto): void {
      if (ciudad.id) {
          this.detalleCityId = ciudad.id;
          this.detalleCityName = ciudad.name || '';
          this.detalleDestinoId = this.getDestinoId(ciudad) || '';
          this.detalleModalVisible = true;
      }
  }

  /**
   * Cierra el modal de detalles
   */
  cerrarModalDetalles(): void {
      this.detalleModalVisible = false;
      this.detalleCityId = 0;
      this.detalleCityName = '';
      this.detalleDestinoId = '';
  }

  /**
   * Limpia el término de búsqueda y los resultados
   */
  clearSearch(): void {
    this.nombreCiudad = '';
    this.nombrePais = '';
    this.nombreRegion = '';
    this.minPoblacion = null;
    this.errorMessage = null;
    this.loadPopularDestinations();
  }

  private loadPopularDestinations(): void {
    this.loading = true;
    this.destinoService.getDestinosPopulares()
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (result) => {
          this.ciudades = result;
        },
        error: (err) => {
          console.error(err);
          this.errorMessage = 'Error al cargar destinos populares.';
        }
      });
  }

  /**
   * Formatea la población
   */
  formatearPoblacion(population: number | undefined): string {
    if (!population) return 'N/A';
    return population.toLocaleString('es-ES');
  }

  /**
   * Formatea las coordenadas
   */
  formatearCoordenadas(lat: number, lng: number): string {
    return `${lat.toFixed(4)}, ${lng.toFixed(4)}`;
  }

  /**
   * Abre el mapa con las coordenadas de la ciudad
   */
  verEnMapa(ciudad: CityDto): void {
    const url = `https://www.google.com/maps/search/?api=1&query=${ciudad.latitude},${ciudad.longitude}`;
    window.open(url, '_blank');
  }
}
