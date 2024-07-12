namespace AuthService.Dto;

public class AuthTokensResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}