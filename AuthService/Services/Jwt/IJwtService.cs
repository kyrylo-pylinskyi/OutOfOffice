using AuthService.Dto;
using AuthService.Dto.Requests;
using AuthService.Models;

namespace AuthService.Services.Jwt;

public interface IJwtService
{
    Task<Result<AuthTokensResponse>> GetUserAuthTokensAsync(ApplicationUser user);
    Task<Result<ApplicationUser>> GetUserFromTokenAsync(string token);
    Task<Result<AuthTokensResponse>> RefreshTokenAsync(TokenRequest request);
}
