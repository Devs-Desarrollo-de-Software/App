import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { ToasterService, Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { DestinoService, CityDto, DestinoDto } from 'src/app/proxy/destinos';
import { finalize } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { CalificacionPromedioComponent } from 'src/app/calificaciones/calificacion-promedio/calificacion-promedio.component';
import { CalificacionModalComponent } from 'src/app/calificaciones/calificacion-modal/calificacion-modal.component';
import { DestinoDetalleModalComponent } from '../destino-detalle-modal/destino-detalle-modal.component';

@Component({
  selector: 'app-destinos-list',
  standalone: true,
  imports: [CommonModule, FormsModule, CoreModule, CalificacionPromedioComponent, CalificacionModalComponent, DestinoDetalleModalComponent],
  templateUrl: './destinos-list.html',
  styleUrls: ['./destinos-list.scss'],
})
export class DestinosList implements OnInit, OnDestroy {
  // Inyección de dependencias usando la nueva sintaxis de inject()
  private readonly destinoService = inject(DestinoService);
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

  /**
   * Subject para implementar debounce en la búsqueda
   */
  private searchSubject = new Subject<void>();

  ngOnInit(): void {
    // Cargar destinos guardados del usuario
    this.cargarDestinosGuardados();

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
          }
        });
      },
      error: (err) => {
        console.error('Error al cargar destinos guardados:', err);
      },
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
    const minPop = this.minPoblacion || 0;
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
      this.modalVisible = false;
      this.modalDestinoId = '';
      this.modalDestinoNombre = '';
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
    // @ts-ignore: El metodo existe en el servicio actualizado manualmente
    this.destinoService.getDestinosPopularesAsync()
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
