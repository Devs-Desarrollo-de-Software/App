import type { AuditedEntityDto } from '@abp/ng.core';

export interface CityDto {
  id: number;
  name?: string;
  country?: string;
  population: number;
  latitude: number;
  longitude: number;
}

export interface CoordenadaDto {
  latitud: number;
  longitud: number;
}

export interface CreateUpdateDestinoDto {
  nombre: string;
  pais: string;
  poblacion: number;
  imagen: string;
  coordenada: CoordenadaDto;
}

export interface DestinoDto extends AuditedEntityDto<string> {
  nombre?: string;
  pais?: string;
  poblacion: number;
  imagen?: string;
  coordenada?: CoordenadaDto;
  apiCityId?: number;
}

export interface CityDetailDto {
  id: number;
  name?: string;
  country?: string;
  countryCode?: string;
  region?: string;
  regionCode?: string;
  latitude: number;
  longitude: number;
  population: number;
  wikiDataId?: string;
  timeZone?: string;
  elevationMeters?: number;
  type?: string;
}
