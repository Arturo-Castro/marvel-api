﻿using AutoMapper;
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
        }
    }
}
