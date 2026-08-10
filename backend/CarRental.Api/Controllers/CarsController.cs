using CarRental.Api.Data;
using CarRental.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CarRental.Api.Controllers;
[ApiController, Route("api/cars")]
public class CarsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Car>>> GetCarsAsync() => Ok(await db.Cars.OrderByDescending(car => car.Id).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Car>> GetCarByIdAsync(int id) => await db.Cars.FindAsync(id) is { } car ? Ok(car) : NotFound();

    [Authorize(Roles = "Admin"), HttpPost]
    public async Task<ActionResult<Car>> CreateCarAsync(Car car)
    {
        db.Cars.Add(car); await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCarByIdAsync), new { id = car.Id }, car);
    }

    [Authorize(Roles = "Admin"), HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCarAsync(int id, Car car)
    {
        if (id != car.Id) return BadRequest();
        if (!await db.Cars.AnyAsync(item => item.Id == id)) return NotFound();
        db.Entry(car).State = EntityState.Modified; await db.SaveChangesAsync(); return NoContent();
    }

    [Authorize(Roles = "Admin"), HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCarAsync(int id)
    {
        var car = await db.Cars.FindAsync(id); if (car is null) return NotFound();
        db.Cars.Remove(car); await db.SaveChangesAsync(); return NoContent();
    }
}
