using System.Security.Claims;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    
    [HttpGet(nameof(GetUserInfo))]
    public async Task<IActionResult> GetUserInfo()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        string email = identity.FindFirst(ClaimTypes.Email)?.Value;

        if(email == null) return BadRequest("User email context is null");
        var user = await _userManager.FindByEmailAsync(email);
        
        return Ok(user);
    }
    
    private static string GetEmail(HttpContext context)
    {
        var identity = context.User.Identity as ClaimsIdentity;
        if (identity != null)
            return identity.FindFirst(ClaimTypes.Email)?.Value;

        return null;
    }
}