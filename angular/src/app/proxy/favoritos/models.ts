import type { DestinoDto } from '../destinos/models';
import type { EntityDto } from '@abp/ng.core';

export interface CrearFavoritoDto {
  destinoId?: string;
}

export interface FavoritoConDestinoDto {
  destino: DestinoDto;
}

export interface FavoritoDto extends EntityDto<string> {
  userId?: string;
  destinoId?: string;
}

export interface ListaFavoritosDto {
  userId?: string;
  favoritos: FavoritoConDestinoDto[];
}
