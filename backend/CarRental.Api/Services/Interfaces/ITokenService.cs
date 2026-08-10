using CarRental.Api.Contracts;
using CarRental.Api.Models;

namespace CarRental.Api.Services.Interfaces;

public interface ITokenService
{
    AuthResponse CreateToken(AppUser user);
}
