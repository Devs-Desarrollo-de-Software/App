using AutoMapper;
using TurisGo.Calificaciones;
using TurisGo.Destinos;
using TurisGo.Experiencias;
using TurisGo.Favoritos;
using TurisGo.Usuarios;

namespace TurisGo;

public class TurisGoApplicationAutoMapperProfile : Profile
{
    public TurisGoApplicationAutoMapperProfile()
    {
        // ---------------- Destinos -----------------------------
        CreateMap<Destino, DestinoDto>();
        CreateMap<CreateUpdateDestinoDto, Destino>()
            .ForMember(dest => dest.Coordenada,
            opt => opt.MapFrom(src => new Coordenada(src.Coordenada.Latitud, src.Coordenada.Longitud)));
        CreateMap<Coordenada, CoordenadaDto>().ReverseMap();

        // --------------- Calificaciones ---------------------
        CreateMap<Calificacion,CalificacionDto>();
        CreateMap<CreateCalificacionDto, Calificacion>();

        // ----------------- Experiencias --------------------
        CreateMap<Experiencia, ExperienciaDto>();
        CreateMap<CreateExperienciaDto, Experiencia>();

        // --------------------- Usuarios ------------------------
        CreateMap<Usuario, UsuarioDto>();
            
        // Mapeo para el perfil público del usuario
        CreateMap<Usuario, PerfilPublicoDto>();

        CreateMap<PreferenciasNotificacion, PreferenciasNotificacionDto>()
            .ReverseMap(); // Permite mapear en ambas direcciones

        // --------------------- Favoritos ------------------------

        CreateMap<Favorito, FavoritoDto>(); 

    }
}
