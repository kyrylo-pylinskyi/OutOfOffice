using AuthService.DTO;
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
    public async Task<IActionResult> Register([FromForm] RegisterModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = _mapper.Map<ApplicationUser>(model);

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            var roleName = model.Role.ToString();
            
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
    public async Task<IActionResult> ResendEmailConfirmation([FromBody] ResendConfirmationEmailModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);
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