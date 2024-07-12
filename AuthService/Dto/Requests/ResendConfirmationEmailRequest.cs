using System.ComponentModel.DataAnnotations;

namespace AuthService.Dto.Requests;

public class ResendConfirmationEmailRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }
}