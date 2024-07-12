using AuthService.Dto;
using AuthService.Dto.Requests;
using AuthService.Models;
using AutoMapper;

namespace AuthService.Profiles;

public class RegisterProfile : Profile
{
    public RegisterProfile()
    {
        CreateMap<RegisterRequest, ApplicationUser>()
            .ForMember(dest => dest.UserName, 
                opt => opt.MapFrom(src => $"{src.Name.ToLower()}.{src.Surname.ToLower()}"))
            .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FullName, 
                opt => opt.MapFrom(src => $"{src.Name} {src.Surname}"))
            .ForMember(dest => dest.IsActive, 
                opt => opt.MapFrom(src => false));
    }
}
