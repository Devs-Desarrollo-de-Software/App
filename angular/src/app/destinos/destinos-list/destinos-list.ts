import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { DestinoService, CityDto } from 'src/app/proxy/destinos';
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

  /**
   * Texto que escribe el usuario para buscar por nombre
   */
  nombreCiudad = '';

  /**
   * Texto que escribe el usuario para buscar por país
   */
  nombrePais = '';

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
  private performSearch(): void {
    const termCiudad = this.nombreCiudad.trim();
    const termPais = this.nombrePais.trim();

    // Si ambos están vacíos, limpiamos resultados
    if (!termCiudad && !termPais) {
      this.ciudades = [];
      this.errorMessage = null;
      return;
    }

    this.loading = true;
    this.errorMessage = null;

    // Si hay nombre de ciudad, buscar por nombre
    if (termCiudad) {
      this.destinoService
        .buscarCiudadesPorNombre(termCiudad)
        .pipe(
          finalize(() => {
            this.loading = false;
          })
        )
        .subscribe({
          next: (result: CityDto[]) => {
            // Si también hay filtro de país, filtrar los resultados
            if (termPais) {
              this.ciudades = result.filter(ciudad =>
                ciudad.country?.toLowerCase().includes(termPais.toLowerCase())
              );
            } else {
              this.ciudades = result || [];
            }

            if (this.ciudades.length === 0 && result.length > 0) {
              this.errorMessage = `No se encontraron ciudades en el país "${termPais}"`;
            }
          },
          error: error => {
            console.error('Error al buscar ciudades:', error);
            this.ciudades = [];
            this.errorMessage = 'Ocurrió un error al buscar ciudades. Intenta nuevamente.';
          },
        });
    } else {
      // Si solo hay país pero no ciudad, mostrar mensaje
      this.loading = false;
      this.errorMessage = 'Por favor, ingresa el nombre de una ciudad para buscar.';
    }
  }

  /**
   * Limpia el término de búsqueda y los resultados
   */
  clearSearch(): void {
    this.nombreCiudad = '';
    this.nombrePais = '';
    this.ciudades = [];
    this.errorMessage = null;
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
