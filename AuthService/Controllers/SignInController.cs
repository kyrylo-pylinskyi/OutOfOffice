using AuthService.Dto.Requests;
using AuthService.Models;
using AuthService.Services.Jwt;
using AuthService.Services.Senders;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignInController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IJwtService _jwtService;

        public SignInController(
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _jwtService = jwtService;
        }

        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login([FromForm] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt. User not found.");
                return BadRequest(ModelState);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Email not confirmed. Please confirm your email before logging in.");
                return BadRequest(ModelState);
            }

            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                ModelState.AddModelError("", "Invalid login attempt. Incorrect password.");
                return BadRequest(ModelState);
            }

            var tokensResult = await _jwtService.GetUserAuthTokensAsync(user);

            if (!tokensResult.IsSuccess)
            {
                ModelState.AddModelError("", tokensResult.Error);
                return BadRequest(ModelState);
            }

            return Ok(tokensResult.Value);
        }

        [HttpPost(nameof(ForgotPassword))]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return BadRequest("User not found");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action(nameof(ResetPassword), "SignIn", new { token, email = user.Email }, Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email, "Reset Password", resetLink);

            return Ok("Password reset link sent.");
        }

        [HttpPost(nameof(ResetPassword))]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return BadRequest("User not found");

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if (result.Succeeded)
                return Ok("Password has been reset.");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return BadRequest(ModelState);
        }
    }
}
