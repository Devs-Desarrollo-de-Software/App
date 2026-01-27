import { mapEnumToOptions } from '@abp/ng.core';

export enum TipoNotificacion {
  CambioDestino = 1,
  NuevoEvento = 2,
  ActualizacionDatos = 3,
  Sistema = 4,
}

export const tipoNotificacionOptions = mapEnumToOptions(TipoNotificacion);
