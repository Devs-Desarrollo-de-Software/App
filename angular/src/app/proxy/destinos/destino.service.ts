import type { CityDetailDto, CityDto, CreateUpdateDestinoDto, DestinoDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DestinoService {
  apiName = 'Default';
  

  buscarCiudadesPorNombre = (nombre: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CityDto[]>({
      method: 'GET',
      url: '/api/app/destino/buscar-ciudad-por-nombre',
      params: { nombre },
    },
    { apiName: this.apiName,...config });
  

  create = (input: CreateUpdateDestinoDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoDto>({
      method: 'POST',
      url: '/api/app/destino',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/destino/${id}`,
    },
    { apiName: this.apiName,...config });
  

  filtrarCiudades = (paisPrefix?: string, poblacionMin?: number, regionPrefix?: string, nombreCiudad?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CityDto[]>({
      method: 'GET',
      url: '/api/app/destino/buscar-ciudades-por-filtro',
      params: { paisPrefix, poblacionMin, regionPrefix, nombreCiudad },
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoDto>({
      method: 'GET',
      url: `/api/app/destino/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getDestinosPopulares = (limit: number = 10, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CityDto[]>({
      method: 'GET',
      url: '/api/app/destino/populares',
      params: { limit },
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<DestinoDto>>({
      method: 'GET',
      url: '/api/app/destino',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  guardarDestinoDesdeApi = (cityId: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoDto>({
      method: 'POST',
      url: `/api/app/destino/guardar-desde-api/${cityId}`,
    },
    { apiName: this.apiName,...config });
  

  obtenerDetalleCiudad = (cityId: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CityDetailDto>({
      method: 'GET',
      url: `/api/app/destino/obtener-info-ciudad/${cityId}`,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateDestinoDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoDto>({
      method: 'PUT',
      url: `/api/app/destino/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
