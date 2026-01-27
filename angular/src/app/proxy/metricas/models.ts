import type { EntityDto } from '@abp/ng.core';

export interface EstadisticasApiExternaDto {
  nombreApi?: string;
  totalLlamadas: number;
  llamadasExitosas: number;
  llamadasFallidas: number;
  tasaExito: number;
  tiempoPromedioMs: number;
  tiempoMinimoMs: number;
  tiempoMaximoMs: number;
  totalResultadosDevueltos?: number;
  fechaDesde?: string;
  fechaHasta?: string;
}

export interface MetricaApiDto extends EntityDto<string> {
  nombreApi?: string;
  endpoint?: string;
  metodoHttp?: string;
  parametrosConsulta?: string;
  codigoEstadoHttp: number;
  tiempoRespuestaMs: number;
  exitosa: boolean;
  mensajeError?: string;
  cantidadResultados?: number;
  fechaHora?: string;
}

export interface ObtenerMetricasInput {
  nombreApi?: string;
  fechaDesde?: string;
  fechaHasta?: string;
  soloFallidas: boolean;
  maxResultCount: number;
}
