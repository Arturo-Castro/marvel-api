using AutoMapper;
using MarvelApp.Domain.Dtos.Character;
using MarvelApp.Domain.Dtos.RescueTeam;
using MarvelApp.Domain.Entities;

namespace MarvelApp.Application.Automapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Character, CharacterBaseDTO>();
            CreateMap<Character, CharacterDetailDTO>()
                .ForMember(dest => dest.RescueTeam,
                    opt => opt.MapFrom(src => src.RescueTeam));
            CreateMap<RescueTeam, RescueTeamBaseDTO>();
            CreateMap<CreateCharacterDTO, Character>();
            CreateMap<Character, CreateCharacterDTO>();
            CreateMap<EditCharacterBaseDTO, Character>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RescueTeamId, opt => opt.Ignore());
            CreateMap<RescueTeam, RescueTeamDetailDTO>();
        }
    }
}
