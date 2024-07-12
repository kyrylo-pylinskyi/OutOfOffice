using System.ComponentModel.DataAnnotations;
using AuthService.Models;

namespace AuthService.Dto.Requests;

public class AssignRoleRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Role is required")]
    public UserRole Role { get; set; }
}