using CarRental.Api.Contracts;
using CarRental.Api.Data;
using CarRental.Api.Exceptions;
using CarRental.Api.Mappings;
using CarRental.Api.Models;
using CarRental.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Api.Services;

public class CarService(AppDbContext db) : ICarService
{
    public async Task<IReadOnlyList<CarResponse>> GetAllAsync()
    {
        var cars = await db.Cars
            .AsNoTracking()
            .OrderByDescending(car => car.Id)
            .ToListAsync();

        return cars.Select(car => car.ToResponse()).ToList();
    }

    public async Task<CarResponse> GetByIdAsync(int id)
    {
        var car = await db.Cars.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id);
        return car?.ToResponse() ?? throw new NotFoundException("Автомобиль не найден.");
    }

    public async Task<CarResponse> CreateAsync(SaveCarRequest request)
    {
        Validate(request);
        var car = new Car();
        request.Apply(car);

        db.Cars.Add(car);
        await db.SaveChangesAsync();
        return car.ToResponse();
    }

    public async Task UpdateAsync(int id, SaveCarRequest request)
    {
        Validate(request);
        var car = await db.Cars.FindAsync(id)
            ?? throw new NotFoundException("Автомобиль не найден.");

        request.Apply(car);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var car = await db.Cars.FindAsync(id)
            ?? throw new NotFoundException("Автомобиль не найден.");
        var hasRentals = await db.Rentals.AnyAsync(rental => rental.CarId == id);
        if (hasRentals)
        {
            throw new ConflictException("Нельзя удалить автомобиль с историей аренд.");
        }

        db.Cars.Remove(car);
        await db.SaveChangesAsync();
    }

    private static void Validate(SaveCarRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Brand)
            || string.IsNullOrWhiteSpace(request.Model)
            || request.Year < 1900
            || request.DailyRate <= 0
            || request.Seats <= 0)
        {
            throw new RequestValidationException("Проверьте обязательные параметры автомобиля.");
        }
    }
}
