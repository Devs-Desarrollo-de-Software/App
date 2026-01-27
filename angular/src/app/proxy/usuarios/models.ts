import type { FrecuenciaNotificacion } from './frecuencia-notificacion.enum';
import type { TipoRol } from './tipo-rol.enum';
import type { EntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export { FrecuenciaNotificacion } from './frecuencia-notificacion.enum';
export { TipoRol } from './tipo-rol.enum';

export interface ActualizarPerfilDto {
  nombreCompleto?: string;
  email?: string;
  fotoPerfilUrl?: string;
}

export interface ActualizarPreferenciasDto {
  recibirEnPantalla: boolean;
  recibirPorEmail: boolean;
  frecuencia: FrecuenciaNotificacion;
}

export interface ActualizarUsuarioDto {
  nombreCompleto?: string;
  email?: string;
  fotoPerfilUrl?: string;
  rol?: TipoRol;
  estaActivo: boolean;
}

export interface CambiarPasswordDto {
  passwordActual: string;
  nuevoPassword: string;
  confirmarNuevoPassword: string;
}

export interface CrearUsuarioDto {
  nombreCompleto: string;
  nombreUsuario: string;
  email: string;
  password: string;
  rol: TipoRol;
  fotoPerfilUrl?: string;
}

export interface EliminarCuentaDto {
  password: string;
}

export interface ObtenerUsuariosInput extends PagedAndSortedResultRequestDto {
  filtro?: string;
  rol?: TipoRol;
  estaActivo?: boolean;
}

export interface PerfilPublicoDto {
  nombreCompleto?: string;
  nombreUsuario?: string;
  fotoPerfilUrl?: string;
}

export interface PreferenciasNotificacionDto {
  recibirEnPantalla: boolean;
  recibirPorEmail: boolean;
  frecuencia?: FrecuenciaNotificacion;
}

export interface UsuarioDto extends EntityDto<string> {
  nombreCompleto?: string;
  nombreUsuario?: string;
  email?: string;
  fotoPerfilUrl?: string;
  preferencias: PreferenciasNotificacionDto;
  rol?: TipoRol;
  estaActivo: boolean;
}
