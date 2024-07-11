using AuthService.DTO;
using AuthService.Models;
using AuthService.Services.Options;

namespace AuthService.Services.Jwt;

public interface IJwtService
{
    Task<SignInResponseModel> GetUserAuthTokensAsync(ApplicationUser user);
}