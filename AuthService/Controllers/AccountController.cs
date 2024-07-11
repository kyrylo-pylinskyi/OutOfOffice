using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using AuthService.Models;
using AuthService.Services.Smtp;
using AuthService.Services.Options;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System;
using AuthService.DTO;
using AutoMapper;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly EmailSender _emailSender;
    private readonly JwtSettings _jwtSettings;

    public AccountController(UserManager<ApplicationUser> userManager, IMapper mapper, EmailSender emailSender, IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _mapper = mapper;
        _emailSender = emailSender;
        _jwtSettings = jwtSettings.Value;
    }
    
    [HttpPost(nameof(Register))]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = _mapper.Map<ApplicationUser>(model);
        user.UserName = model.Email;

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);
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
}
