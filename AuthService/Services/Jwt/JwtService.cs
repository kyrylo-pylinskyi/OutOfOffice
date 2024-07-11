using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.DTO;
using AuthService.Models;
using AuthService.Services.Jwt;
using AuthService.Services.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class JwtService : IJwtService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtSettings _jwtSettings;

    public JwtService(UserManager<ApplicationUser> userManager, IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<SignInResponseModel> GetUserAuthTokensAsync(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email)
        };
        
        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var accessTokenClaims = new ClaimsIdentity(claims);

        var refreshTokenClaims = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        });

        return new SignInResponseModel
        {
            AccessToken = GenerateJwtToken(accessTokenClaims, TimeSpan.FromMinutes(_jwtSettings.AccessTokenExpirationMinutes)),
            RefreshToken = GenerateJwtToken(refreshTokenClaims, TimeSpan.FromDays(_jwtSettings.RefreshTokensExpirationDays))
        };
    }

    private string GenerateJwtToken(ClaimsIdentity claimsIdentity, TimeSpan expiration)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = _jwtSettings.Audience,
            Issuer = _jwtSettings.Issuer,
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.Add(expiration),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }
}
