using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO;

public class ResendConfirmationEmailModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }
}