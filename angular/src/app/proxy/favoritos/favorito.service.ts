import type { CrearFavoritoDto, FavoritoDto, ListaFavoritosDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class FavoritoService {
  apiName = 'Default';
  

  agregarFavorito = (input: CrearFavoritoDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, FavoritoDto>({
      method: 'POST',
      url: '/api/app/favorito/agregar-favorito',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (favoritoId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: '/api/app/favorito',
      params: { favoritoId },
    },
    { apiName: this.apiName,...config });
  

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ListaFavoritosDto>({
      method: 'GET',
      url: '/api/app/favorito',
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
