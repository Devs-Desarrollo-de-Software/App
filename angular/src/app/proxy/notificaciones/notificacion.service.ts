import type { GetNotificacionesInput, NotificacionDto, NotificacionResultDto } from './models';
import type { TipoNotificacion } from './tipo-notificacion.enum';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class NotificacionService {
  apiName = 'Default';
  

  getConteoNoLeidas = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, number>({
      method: 'GET',
      url: '/api/app/notificacion/conteo-no-leidas',
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetNotificacionesInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<NotificacionDto>>({
      method: 'GET',
      url: '/api/app/notificacion',
      params: { soloNoLeidas: input.soloNoLeidas, tipoFiltro: input.tipoFiltro, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  marcarComoLeida = (id: string, leida: boolean, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/notificacion/${id}/marcar-como-leida`,
      params: { leida },
    },
    { apiName: this.apiName,...config });
  

  notificarCambioDestino = (destinoId: string, titulo: string, mensaje: string, tipo: TipoNotificacion, config?: Partial<Rest.Config>) =>
    this.restService.request<any, NotificacionResultDto>({
      method: 'POST',
      url: `/api/app/notificacion/notificar-cambio-destino/${destinoId}`,
      params: { titulo, mensaje, tipo },
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
