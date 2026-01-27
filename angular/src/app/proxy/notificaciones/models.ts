import type { EntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { TipoNotificacion } from './tipo-notificacion.enum';

export interface GetNotificacionesInput extends PagedAndSortedResultRequestDto {
  soloNoLeidas?: boolean;
  tipoFiltro?: TipoNotificacion;
}

export interface NotificacionDto extends EntityDto<string> {
  destinoId?: string;
  titulo?: string;
  mensaje?: string;
  tipo?: TipoNotificacion;
  leida: boolean;
  fechaCreacion?: string;
  nombreDestino?: string;
}

export interface NotificacionResultDto {
  usuariosNotificados: number;
  nombreDestino?: string;
  tipo?: TipoNotificacion;
  mensaje?: string;
}
