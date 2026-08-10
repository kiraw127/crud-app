using System.ComponentModel.DataAnnotations;

namespace CarRental.Api.Dtos;

public record RegisterRequestDto(
    [Required] string Name,
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password);

public record LoginRequestDto(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public record AuthResponseDto(string Token, string Name, string Role);
