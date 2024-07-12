using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Dto;
using AuthService.Dto.Requests;
using AuthService.Models;
using AuthService.Services.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services.Jwt;

public class JwtService : IJwtService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtSettings _jwtSettings;

    public JwtService(UserManager<ApplicationUser> userManager, JwtSettings jwtSettings)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings;
    }

    public async Task<Result<AuthTokensResponse>> GetUserAuthTokensAsync(ApplicationUser user)
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

        var response = new AuthTokensResponse
        {
            AccessToken = GenerateJwtToken(accessTokenClaims, TimeSpan.FromMinutes(_jwtSettings.AccessTokenExpirationMinutes)),
            RefreshToken = GenerateJwtToken(refreshTokenClaims, TimeSpan.FromDays(_jwtSettings.RefreshTokensExpirationDays))
        };

        return Result<AuthTokensResponse>.Success(response);
    }

    public async Task<Result<ApplicationUser>> GetUserFromTokenAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                return Result<ApplicationUser>.Success(user);
            }
            return Result<ApplicationUser>.Failure("User not found");
        }
        catch (Exception ex)
        {
            return Result<ApplicationUser>.Failure("Invalid token");
        }
    }

    public async Task<Result<AuthTokensResponse>> RefreshTokenAsync(TokenRequest request)
    {
        var userResult = await GetUserFromTokenAsync(request.Token);
        if (!userResult.IsSuccess)
        {
            return Result<AuthTokensResponse>.Failure(userResult.Error);
        }

        var tokensResult = await GetUserAuthTokensAsync(userResult.Value);
        if (!tokensResult.IsSuccess)
        {
            return Result<AuthTokensResponse>.Failure(tokensResult.Error);
        }

        return tokensResult;
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
        return tokenHandler.WriteToken(token);
    }
}