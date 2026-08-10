using CarRental.Api.Dtos;
using CarRental.Api.Models;

namespace CarRental.Api.Services.Interfaces;

public interface ITokenService
{
    AuthResponseDto CreateToken(AppUser user);
}
