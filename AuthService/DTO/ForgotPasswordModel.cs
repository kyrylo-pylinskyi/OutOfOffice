using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO;

public class ForgotPasswordModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }
}