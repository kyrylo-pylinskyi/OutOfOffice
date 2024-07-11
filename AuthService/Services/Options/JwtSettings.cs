namespace AuthService.Services.Options;

public class JwtSettings
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Key { get; set; }
    public double AccessTokenExpirationMinutes { get; set; }
    public int RefreshTokensExpirationDays { get; set; }
}
