using AutoMapper;
using TurisGo.Destinos;

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
    }
}
