using CarRental.Api.Data;
using CarRental.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CarRental.Api.Controllers;
[ApiController, Route("api/[controller]")]
public class RentalsController(AppDbContext db) : ControllerBase
{
    [Authorize(Roles = "Admin")][HttpGet] public async Task<ActionResult<IEnumerable<Rental>>> Get() => Ok(await db.Rentals.Include(x => x.Car).OrderByDescending(x => x.Id).ToListAsync());
    [Authorize][HttpGet("mine")] public async Task<ActionResult<IEnumerable<Rental>>> Mine() { var email=User.FindFirstValue(ClaimTypes.Email); var user=await db.Users.SingleOrDefaultAsync(x=>x.Email==email); if(user is null)return Unauthorized(); return Ok(await db.Rentals.Include(x=>x.Car).Where(x=>x.UserId==user.Id).OrderByDescending(x=>x.Id).ToListAsync()); }
    [Authorize][HttpPost] public async Task<IActionResult> Create(Rental rental) { var email=User.FindFirstValue(ClaimTypes.Email); var user=await db.Users.SingleOrDefaultAsync(x=>x.Email==email); if(user is null)return Unauthorized(); var car = await db.Cars.FindAsync(rental.CarId); if (car is null || !car.IsAvailable) return BadRequest("Автомобиль недоступен"); if (rental.EndDate <= rental.StartDate) return BadRequest("Дата окончания должна быть позже даты начала"); var days = rental.EndDate.DayNumber - rental.StartDate.DayNumber; rental.UserId=user.Id; rental.CustomerName=user.Name; rental.TotalPrice = days * car.DailyRate; car.IsAvailable = false; db.Rentals.Add(rental); await db.SaveChangesAsync(); return Ok(new { rental.Id, rental.CarId, rental.UserId, rental.CustomerName, rental.Phone, rental.StartDate, rental.EndDate, rental.TotalPrice, rental.Status }); }
    [Authorize][HttpDelete("mine/{id:int}")] public async Task<IActionResult> DeleteMine(int id) { var email=User.FindFirstValue(ClaimTypes.Email); var user=await db.Users.SingleOrDefaultAsync(x=>x.Email==email); if(user is null)return Unauthorized(); var rental=await db.Rentals.SingleOrDefaultAsync(x=>x.Id==id&&x.UserId==user.Id); if(rental is null)return NotFound(); var car=await db.Cars.FindAsync(rental.CarId); if(car is not null)car.IsAvailable=true; db.Rentals.Remove(rental); await db.SaveChangesAsync(); return NoContent(); }
    [Authorize(Roles = "Admin")][HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, Rental value) { var rental = await db.Rentals.FindAsync(id); if (rental is null) return NotFound(); rental.CustomerName=value.CustomerName; rental.Phone=value.Phone; rental.StartDate=value.StartDate; rental.EndDate=value.EndDate; rental.Status=value.Status; await db.SaveChangesAsync(); return NoContent(); }
    [Authorize(Roles = "Admin")][HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id) { var rental=await db.Rentals.FindAsync(id); if(rental is null)return NotFound(); var car=await db.Cars.FindAsync(rental.CarId); if(car is not null)car.IsAvailable=true; db.Rentals.Remove(rental); await db.SaveChangesAsync(); return NoContent(); }
}
