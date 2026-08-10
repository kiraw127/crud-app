using CarRental.Api.Dtos;
using CarRental.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Controllers;

[ApiController, Route("api/cars")]
public class CarsController(ICarService carService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CarResponseDto>>> GetCarsAsync(
        [FromQuery] CarQueryDto query)
    {
        return Ok(await carService.GetAllAsync(query));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CarResponseDto>> GetCarByIdAsync(int id)
    {
        return Ok(await carService.GetByIdAsync(id));
    }

    [Authorize(Roles = "Admin"), HttpPost]
    public async Task<ActionResult<CarResponseDto>> CreateCarAsync(SaveCarRequestDto request)
    {
        var car = await carService.CreateAsync(request);
        return CreatedAtAction(nameof(GetCarByIdAsync), new { id = car.Id }, car);
    }

    [Authorize(Roles = "Admin"), HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCarAsync(int id, SaveCarRequestDto request)
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
