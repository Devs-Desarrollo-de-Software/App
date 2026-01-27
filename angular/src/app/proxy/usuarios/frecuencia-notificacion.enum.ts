import { mapEnumToOptions } from '@abp/ng.core';

export enum FrecuenciaNotificacion {
  Inmediata = 0,
  Semanal = 1,
}

export const frecuenciaNotificacionOptions = mapEnumToOptions(FrecuenciaNotificacion);
