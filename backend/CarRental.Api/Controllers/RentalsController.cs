using CarRental.Api.Data;
using CarRental.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarRental.Api.Controllers;

[ApiController, Route("api/rentals")]
public class RentalsController(AppDbContext db) : ControllerBase
{
    [Authorize(Roles = "Admin"), HttpGet]
    public async Task<ActionResult<IEnumerable<Rental>>> GetRentalsAsync()
    {
        var rentals = await db.Rentals
            .Include(rental => rental.Car)
            .OrderByDescending(rental => rental.Id)
            .ToListAsync();

        return Ok(rentals);
    }

    [Authorize, HttpGet("me")]
    public async Task<ActionResult<IEnumerable<Rental>>> GetMyRentalsAsync()
    {
        var currentUser = await FindCurrentUserAsync();
        if (currentUser is null) return Unauthorized();

        var rentals = await db.Rentals
            .Include(rental => rental.Car)
            .Where(rental => rental.UserId == currentUser.Id)
            .OrderByDescending(rental => rental.Id)
            .ToListAsync();

        return Ok(rentals);
    }

    [Authorize, HttpPost]
    public async Task<IActionResult> CreateRentalAsync(Rental rental)
    {
        var currentUser = await FindCurrentUserAsync();
        if (currentUser is null) return Unauthorized();

        var car = await db.Cars.FindAsync(rental.CarId);
        if (car is null || !car.IsAvailable) return BadRequest("Автомобиль недоступен");
        if (rental.EndDate <= rental.StartDate)
            return BadRequest("Дата окончания должна быть позже даты начала");

        var rentalDays = rental.EndDate.DayNumber - rental.StartDate.DayNumber;
        rental.UserId = currentUser.Id;
        rental.CustomerName = currentUser.Name;
        rental.TotalPrice = rentalDays * car.DailyRate;
        car.IsAvailable = false;

        db.Rentals.Add(rental);
        await db.SaveChangesAsync();

        return Ok(new
        {
            rental.Id,
            rental.CarId,
            rental.UserId,
            rental.CustomerName,
            rental.Phone,
            rental.StartDate,
            rental.EndDate,
            rental.TotalPrice,
            rental.Status
        });
    }

    [Authorize, HttpDelete("me/{id:int}")]
    public async Task<IActionResult> CancelMyRentalAsync(int id)
    {
        var currentUser = await FindCurrentUserAsync();
        if (currentUser is null) return Unauthorized();

        var rental = await db.Rentals.SingleOrDefaultAsync(
            item => item.Id == id && item.UserId == currentUser.Id);
        if (rental is null) return NotFound();

        await DeleteRentalAndReleaseCarAsync(rental);
        return NoContent();
    }

    [Authorize(Roles = "Admin"), HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateRentalAsync(int id, Rental updatedRental)
    {
        var rental = await db.Rentals.FindAsync(id);
        if (rental is null) return NotFound();

        rental.CustomerName = updatedRental.CustomerName;
        rental.Phone = updatedRental.Phone;
        rental.StartDate = updatedRental.StartDate;
        rental.EndDate = updatedRental.EndDate;
        rental.Status = updatedRental.Status;
        await db.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Admin"), HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteRentalAsync(int id)
    {
        var rental = await db.Rentals.FindAsync(id);
        if (rental is null) return NotFound();

        await DeleteRentalAndReleaseCarAsync(rental);
        return NoContent();
    }

    private async Task<AppUser?> FindCurrentUserAsync()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        return await db.Users.SingleOrDefaultAsync(user => user.Email == email);
    }

    private async Task DeleteRentalAndReleaseCarAsync(Rental rental)
    {
        var car = await db.Cars.FindAsync(rental.CarId);
        if (car is not null) car.IsAvailable = true;

        db.Rentals.Remove(rental);
        await db.SaveChangesAsync();
    }
}
