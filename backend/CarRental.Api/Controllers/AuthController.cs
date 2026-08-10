using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CarRental.Api.Contracts;
using CarRental.Api.Data;
using CarRental.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CarRental.Api.Controllers;
[ApiController, Route("api/auth")]
public class AuthController(AppDbContext db, IConfiguration config) : ControllerBase
{
    private readonly PasswordHasher<AppUser> hasher = new();

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(request.Name) || !email.Contains('@') || request.Password.Length < 6)
            return BadRequest("Укажите имя, корректный email и пароль минимум из 6 символов.");
        if (await db.Users.AnyAsync(x => x.Email == email)) return Conflict("Этот email уже зарегистрирован.");
        var user = new AppUser { Name = request.Name.Trim(), Email = email, Role = "Customer" };
        user.PasswordHash = hasher.HashPassword(user, request.Password);
        db.Users.Add(user); await db.SaveChangesAsync();
        return Ok(CreateResponse(user));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await db.Users.SingleOrDefaultAsync(x => x.Email == request.Email.Trim().ToLowerInvariant());
        if (user is null || hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            return Unauthorized("Неверный email или пароль.");
        return Ok(CreateResponse(user));
    }

    private AuthResponse CreateResponse(AppUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var token = new JwtSecurityToken(claims: [new(ClaimTypes.Name, user.Name), new(ClaimTypes.Email, user.Email), new(ClaimTypes.Role, user.Role)], expires: DateTime.UtcNow.AddHours(12), signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new(new JwtSecurityTokenHandler().WriteToken(token), user.Name, user.Role);
    }
}
