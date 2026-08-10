using CarRental.Api.Dtos;

namespace CarRental.Api.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
}
