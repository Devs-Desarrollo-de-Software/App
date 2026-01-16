import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { DestinoService, CityDto, DestinoDto } from 'src/app/proxy/destinos';
import { finalize } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-destinos-list',
  standalone: true,
  imports: [CommonModule, FormsModule, CoreModule],
  templateUrl: './destinos-list.html',
  styleUrls: ['./destinos-list.scss'],
})
export class DestinosList implements OnInit, OnDestroy {
  // Inyección de dependencias usando la nueva sintaxis de inject()
  private readonly destinoService = inject(DestinoService);
  private readonly toaster = inject(ToasterService);

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
   * Subject para implementar debounce en la búsqueda
   */
  private searchSubject = new Subject<void>();

  ngOnInit(): void {
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
