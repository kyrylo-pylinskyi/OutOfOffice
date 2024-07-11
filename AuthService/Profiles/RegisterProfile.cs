using AuthService.DTO;
using AuthService.Models;
using AutoMapper;

namespace AuthService.Profiles;

public class RegisterProfile : Profile
{
    public RegisterProfile()
    {
        CreateMap<RegisterModel, ApplicationUser>()
            .ForMember(dest => dest.FullName, 
                opt => opt.MapFrom(src => $"{src.Name} {src.Surname}"))
            .ForMember(dest => dest.IsActive, 
                opt => opt.MapFrom(src => false));
    }
}
