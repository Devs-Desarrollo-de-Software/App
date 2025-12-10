import type { CalificacionDto, CreateUpdateCalificacionDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CalificacionService {
  apiName = 'Default';
  

  create = (input: CreateUpdateCalificacionDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CalificacionDto>({
      method: 'POST',
      url: '/api/app/calificacion',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/calificacion/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CalificacionDto>({
      method: 'GET',
      url: `/api/app/calificacion/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<CalificacionDto>>({
      method: 'GET',
      url: '/api/app/calificacion',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateCalificacionDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CalificacionDto>({
      method: 'PUT',
      url: `/api/app/calificacion/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
