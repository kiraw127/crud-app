using CarRental.Api.Dtos;
using CarRental.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Controllers;

[ApiController, Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        return Ok(await authService.RegisterAsync(request));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> LoginAsync(LoginRequestDto request)
    {
        return Ok(await authService.LoginAsync(request));
    }
}
