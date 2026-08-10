using CarRental.Api.Dtos;
using CarRental.Api.Data;
using CarRental.Api.Exceptions;
using CarRental.Api.Models;
using CarRental.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Api.Services;

public class AuthService(AppDbContext db, ITokenService tokenService) : IAuthService
{
    private readonly PasswordHasher<AppUser> passwordHasher = new();

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        ValidateRegistration(request);
        var email = NormalizeEmail(request.Email);

        if (await db.Users.AnyAsync(user => user.Email == email))
        {
            throw new ConflictException("Этот email уже зарегистрирован.");
        }

        var user = new AppUser
        {
            Name = request.Name.Trim(),
            Email = email,
            Role = "Customer"
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        db.Users.Add(user);
        await db.SaveChangesAsync();
        return tokenService.CreateToken(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var email = NormalizeEmail(request.Email);
        var user = await db.Users.SingleOrDefaultAsync(item => item.Email == email);
        if (user is null) throw new UnauthorizedAccessException("Неверный email или пароль.");

        var passwordResult = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);
        if (passwordResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Неверный email или пароль.");
        }

        return tokenService.CreateToken(user);
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static void ValidateRegistration(RegisterRequestDto request)
    {
        var email = NormalizeEmail(request.Email);
        if (string.IsNullOrWhiteSpace(request.Name)
            || !email.Contains('@')
            || request.Password.Length < 6)
        {
            throw new RequestValidationException(
                "Укажите имя, корректный email и пароль минимум из 6 символов.");
        }
    }
}
