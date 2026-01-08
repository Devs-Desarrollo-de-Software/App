using AutoMapper;
using TurisGo.Calificaciones;
using TurisGo.Destinos;
using TurisGo.Experiencias;
using TurisGo.Usuarios;

namespace TurisGo;

public class TurisGoApplicationAutoMapperProfile : Profile
{
    public TurisGoApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<Destino, DestinoDto>();
        CreateMap<CreateUpdateDestinoDto, Destino>()
            .ForMember(dest => dest.Coordenada,
            opt => opt.MapFrom(src => new Coordenada(src.Coordenada.Latitud, src.Coordenada.Longitud)));
        CreateMap<Coordenada, CoordenadaDto>().ReverseMap();

        CreateMap<Calificacion,CalificacionDto>();
        CreateMap<CreateCalificacionDto, Calificacion>();

        CreateMap<Experiencia, ExperienciaDto>();
        CreateMap<CreateExperienciaDto, Experiencia>();

        CreateMap<Usuario, UsuarioDto>();
            
        // Mapeo para el perfil público del usuario
        CreateMap<Usuario, PerfilPublicoDto>();

        CreateMap<PreferenciasNotificacion, PreferenciasNotificacionDto>()
            .ReverseMap(); // Permite mapear en ambas direcciones

    }
}
