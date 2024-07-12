using AuthService.Dto;
using AuthService.Dto.Requests;
using AuthService.Services.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;

    public AccountController(IJwtService jwtService, IMapper mapper)
    {
        _jwtService = jwtService;
        _mapper = mapper;
    }
    
    [HttpPost(nameof(GetUserFromToken))]
    public async Task<IActionResult> GetUserFromToken([FromBody] TokenRequest tokenRequest)
    {
        var result = await _jwtService.GetUserFromTokenAsync(tokenRequest.Token);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        var userInfoResponse = _mapper.Map<UserInfoResonse>(result.Value);
        
        return Ok(userInfoResponse);
    }

    [HttpPost(nameof(RefreshToken))]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
    {
        var result = await _jwtService.RefreshTokenAsync(tokenRequest);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}