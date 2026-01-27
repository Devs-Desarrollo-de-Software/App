import type { AuditedEntityDto } from '@abp/ng.core';

export interface CalificacionDto extends AuditedEntityDto<string> {
  puntuacion: number;
  comentario?: string;
  destinoId?: string;
  userId?: string;
  destinoNombre?: string;
}

export interface ComentarioDto {
  puntuacion: number;
  comentario?: string;
  creationTime?: string;
}

export interface CreateCalificacionDto {
  destinoId: string;
  puntuacion: number;
  comentario?: string;
}

export interface ListarComentariosDto {
  destinoId?: string;
  comentarios: ComentarioDto[];
}

export interface PromedioCalificacionDto {
  destinoId?: string;
  promedioCalificacion: number;
  totalCalificaciones: number;
}

export interface UpdateCalificacionDto {
  puntuacion: number;
  comentario?: string;
}
