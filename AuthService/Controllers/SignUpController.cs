using AuthService.Dto.Requests;
using AuthService.Models;
using AuthService.Services.Senders;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SignUpController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IEmailSender _emailSender;
    private readonly RoleManager<IdentityRole> _roleManager;

    public SignUpController(
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        IEmailSender emailSender,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _mapper = mapper;
        _emailSender = emailSender;
        _roleManager = roleManager;
    }
    
    [HttpPost(nameof(Register))]
    public async Task<IActionResult> Register([FromForm] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _userManager.FindByEmailAsync(request.Email) != null)
        {
            ModelState.AddModelError("", $"Email address {request.Email} is reserved");
            return BadRequest(ModelState);
        }

        var user = _mapper.Map<ApplicationUser>(request);

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            var roleName = request.Role.ToString();
            
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            await _userManager.AddToRoleAsync(user, roleName);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "SignUp", new { token, email = user.Email }, Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email, "Confirm your email", confirmationLink);
            return Ok("Registration successful. Please confirm your email.");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return BadRequest(ModelState);
    }
    
    [HttpGet(nameof(ConfirmEmail))]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return BadRequest("Invalid Email Confirmation Request");

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded)
        {
            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            return Ok("Email confirmed successfully! User is now active.");
        }

        return BadRequest("Email confirmation failed");
    }
    
    [HttpPost(nameof(ResendEmailConfirmation))]
    public async Task<IActionResult> ResendEmailConfirmation([FromBody] ResendConfirmationEmailRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return BadRequest("User not found.");

        if (await _userManager.IsEmailConfirmedAsync(user))
            return BadRequest("Email is already confirmed.");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = Url.Action(nameof(ConfirmEmail), "SignUp", new { token, email = user.Email }, Request.Scheme);
        await _emailSender.SendEmailAsync(user.Email, "Confirm your email", confirmationLink);

        return Ok("Confirmation email resent.");
    }
}