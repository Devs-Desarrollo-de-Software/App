import { mapEnumToOptions } from '@abp/ng.core';

export enum TipoValoracion {
  Positiva = 0,
  Neutra = 1,
  Negativa = 2,
}

export const tipoValoracionOptions = mapEnumToOptions(TipoValoracion);
