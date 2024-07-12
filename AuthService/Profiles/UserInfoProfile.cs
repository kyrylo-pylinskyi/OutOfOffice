using AuthService.Dto;
using AuthService.Models;
using AutoMapper;

namespace AuthService.Profiles;

public class UserInfoProfile : Profile
{
    public UserInfoProfile()
    {
        CreateMap<ApplicationUser, UserInfoResonse>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.IsActive,
                opt => opt.MapFrom(src => src.IsActive));
    }
}