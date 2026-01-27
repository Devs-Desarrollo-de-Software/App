import type { TipoValoracion } from './tipo-valoracion.enum';
import type { AuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CreateExperienciaDto {
  destinoId: string;
  titulo: string;
  descripcion: string;
  valoracion: TipoValoracion;
}

export interface ExperienciaDto extends AuditedEntityDto<string> {
  userId?: string;
  destinoId?: string;
  titulo?: string;
  descripcion?: string;
  valoracion?: TipoValoracion;
}

export interface ExperienciaPropiaDto {
  nombreUsuario?: string;
  titulo?: string;
  descripcion?: string;
  valoracion?: TipoValoracion;
}

export interface GetExperienciasListDto extends PagedAndSortedResultRequestDto {
  valoracion?: TipoValoracion;
  palabraClave?: string;
}

export interface ListarExperienciasDto {
  destinoId?: string;
  experiencias: ExperienciaPropiaDto[];
}

export interface UpdateExperienciaDto {
  titulo: string;
  descripcion: string;
  valoracion: TipoValoracion;
}
