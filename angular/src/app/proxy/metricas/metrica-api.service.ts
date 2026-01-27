import type { EstadisticasApiExternaDto, MetricaApiDto, ObtenerMetricasInput } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class MetricaApiService {
  apiName = 'Default';
  

  obtenerEstadisticas = (nombreApi: string, fechaDesde?: string, fechaHasta?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, EstadisticasApiExternaDto>({
      method: 'GET',
      url: '/api/app/metrica-api/estadisticas',
      params: { nombreApi, fechaDesde, fechaHasta },
    },
    { apiName: this.apiName,...config });
  

  obtenerMetricas = (input: ObtenerMetricasInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, MetricaApiDto[]>({
      method: 'GET',
      url: '/api/app/metrica-api/metricas',
      params: { nombreApi: input.nombreApi, fechaDesde: input.fechaDesde, fechaHasta: input.fechaHasta, soloFallidas: input.soloFallidas, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  obtenerMetricasFallidas = (nombreApi?: string, fechaDesde?: string, fechaHasta?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, MetricaApiDto[]>({
      method: 'GET',
      url: '/api/app/metrica-api/metricas-fallidas',
      params: { nombreApi, fechaDesde, fechaHasta },
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
