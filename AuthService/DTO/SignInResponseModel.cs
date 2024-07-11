namespace AuthService.DTO;

public class SignInResponseModel
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}