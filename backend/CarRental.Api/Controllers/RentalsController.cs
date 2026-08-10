using System.Security.Claims;
using CarRental.Api.Dtos;
using CarRental.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Controllers;

[ApiController, Authorize, Route("api/rentals")]
public class RentalsController(IRentalService rentalService) : ControllerBase
{
    [Authorize(Roles = "Admin"), HttpGet]
    public async Task<ActionResult<IReadOnlyList<RentalResponseDto>>> GetRentalsAsync()
    {
        return Ok(await rentalService.GetAllAsync());
    }

    [HttpGet("me")]
    public async Task<ActionResult<IReadOnlyList<RentalResponseDto>>> GetMyRentalsAsync()
    {
        return Ok(await rentalService.GetForUserAsync(GetCurrentUserEmail()));
    }

    [HttpPost]
    public async Task<ActionResult<RentalResponseDto>> CreateRentalAsync(CreateRentalRequestDto request)
    {
        var rental = await rentalService.CreateAsync(GetCurrentUserEmail(), request);
        return Created($"api/rentals/{rental.Id}", rental);
    }

    [HttpDelete("me/{id:int}")]
    public async Task<IActionResult> CancelMyRentalAsync(int id)
    {
        await rentalService.CancelForUserAsync(GetCurrentUserEmail(), id);
        return NoContent();
    }

    [Authorize(Roles = "Admin"), HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateRentalAsync(int id, UpdateRentalRequestDto request)
    {
        await rentalService.UpdateAsync(id, request);
        return NoContent();
    }

    [Authorize(Roles = "Admin"), HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteRentalAsync(int id)
    {
        await rentalService.DeleteAsync(id);
        return NoContent();
    }

    private string GetCurrentUserEmail()
    {
        return User.FindFirstValue(ClaimTypes.Email)
            ?? throw new UnauthorizedAccessException("Email отсутствует в токене.");
    }
}
