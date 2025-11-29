using AutoMapper;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;

namespace RangoAgil.API.Profiles;
    public class RangoAgilProfile: Profile
    {
        public RangoAgilProfile()
        {
            CreateMap<Rango, RangoDTO>().ReverseMap();
            CreateMap<RangoForCreateDTO, Rango>().ReverseMap();
        CreateMap<Ingrediente, IngredienteDTO>()
        .ForMember(d => d.RangoID, o => o.MapFrom(s => s.Rangos.First().Id));

        CreateMap<Rango, RangoForUpdateDTO>().ReverseMap();


    }
    }
 