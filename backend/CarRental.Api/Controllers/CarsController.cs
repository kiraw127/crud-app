using CarRental.Api.Contracts;
using CarRental.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Controllers;

[ApiController, Route("api/cars")]
public class CarsController(ICarService carService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CarResponse>>> GetCarsAsync()
    {
        return Ok(await carService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CarResponse>> GetCarByIdAsync(int id)
    {
        return Ok(await carService.GetByIdAsync(id));
    }

    [Authorize(Roles = "Admin"), HttpPost]
    public async Task<ActionResult<CarResponse>> CreateCarAsync(SaveCarRequest request)
    {
        var car = await carService.CreateAsync(request);
        return CreatedAtAction(nameof(GetCarByIdAsync), new { id = car.Id }, car);
    }

    [Authorize(Roles = "Admin"), HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCarAsync(int id, SaveCarRequest request)
    {
        await carService.UpdateAsync(id, request);
        return NoContent();
    }

    [Authorize(Roles = "Admin"), HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCarAsync(int id)
    {
        await carService.DeleteAsync(id);
        return NoContent();
    }
}
