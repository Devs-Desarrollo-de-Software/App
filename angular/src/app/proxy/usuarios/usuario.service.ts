import type { ActualizarPerfilDto, ActualizarPreferenciasDto, ActualizarUsuarioDto, CambiarPasswordDto, CrearUsuarioDto, EliminarCuentaDto, ObtenerUsuariosInput, PerfilPublicoDto, UsuarioDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class UsuarioService {
  apiName = 'Default';
  

  actualizarPerfil = (input: ActualizarPerfilDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, UsuarioDto>({
      method: 'PUT',
      url: '/api/app/usuario/actualizar-perfil',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  actualizarPreferencias = (input: ActualizarPreferenciasDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, UsuarioDto>({
      method: 'PUT',
      url: '/api/app/usuario/actualizar-preferencias',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  actualizarUsuario = (id: string, input: ActualizarUsuarioDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, UsuarioDto>({
      method: 'PUT',
      url: `/api/app/usuario/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  cambiarPassword = (input: CambiarPasswordDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/usuario/cambiar-password',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  eliminarCuentaPropiaByInput = (input: EliminarCuentaDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: '/api/app/usuario/eliminar-cuenta-propia',
      params: { password: input.password },
    },
    { apiName: this.apiName,...config });
  

  eliminarUsuario = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/usuario/${id}`,
    },
    { apiName: this.apiName,...config });
  

  obtenerPerfilActual = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, UsuarioDto>({
      method: 'GET',
      url: '/api/app/usuario/obtener-perfil-actual',
    },
    { apiName: this.apiName,...config });
  

  obtenerPerfilPublico = (nombreUsuario: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PerfilPublicoDto>({
      method: 'GET',
      url: '/api/app/usuario/obtener-perfil-publico',
      params: { nombreUsuario },
    },
    { apiName: this.apiName,...config });
  

  obtenerTodosUsuarios = (input: ObtenerUsuariosInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<UsuarioDto>>({
      method: 'GET',
      url: '/api/app/usuario/obtener-todos-usuarios',
      params: { filtro: input.filtro, rol: input.rol, estaActivo: input.estaActivo, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  obtenerUsuarioPorId = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, UsuarioDto>({
      method: 'GET',
      url: `/api/app/usuario/${id}`,
    },
    { apiName: this.apiName,...config });
  

  reactivarUsuario = (input: CrearUsuarioDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, UsuarioDto>({
      method: 'POST',
      url: '/api/app/usuario/reactivar',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  registrarUsuario = (input: CrearUsuarioDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, UsuarioDto>({
      method: 'POST',
      url: '/api/app/usuario/registrar-usuario',
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
