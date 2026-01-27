import { mapEnumToOptions } from '@abp/ng.core';

export enum TipoRol {
  Usuario = 0,
  Administrador = 1,
}

export const tipoRolOptions = mapEnumToOptions(TipoRol);
