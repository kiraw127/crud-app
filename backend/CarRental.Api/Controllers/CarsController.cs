using CarRental.Api.Data;
using CarRental.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CarRental.Api.Controllers;
[ApiController, Route("api/[controller]")]
public class CarsController(AppDbContext db) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IEnumerable<Car>>> Get() => Ok(await db.Cars.OrderByDescending(x => x.Id).ToListAsync());
    [HttpGet("{id:int}")] public async Task<ActionResult<Car>> Get(int id) => await db.Cars.FindAsync(id) is { } car ? Ok(car) : NotFound();
    [Authorize(Roles = "Admin")][HttpPost] public async Task<ActionResult<Car>> Create(Car car) { db.Cars.Add(car); await db.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = car.Id }, car); }
    [Authorize(Roles = "Admin")][HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, Car car) { if (id != car.Id) return BadRequest(); if (!await db.Cars.AnyAsync(x => x.Id == id)) return NotFound(); db.Entry(car).State = EntityState.Modified; await db.SaveChangesAsync(); return NoContent(); }
    [Authorize(Roles = "Admin")][HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id) { var car = await db.Cars.FindAsync(id); if (car is null) return NotFound(); db.Cars.Remove(car); await db.SaveChangesAsync(); return NoContent(); }
}
