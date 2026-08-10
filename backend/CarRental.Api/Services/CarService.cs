using CarRental.Api.Dtos;
using CarRental.Api.Data;
using CarRental.Api.Exceptions;
using CarRental.Api.Mappings;
using CarRental.Api.Models;
using CarRental.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Api.Services;

public class CarService(AppDbContext db) : ICarService
{
    public async Task<IReadOnlyList<CarResponseDto>> GetAllAsync(CarQueryDto query)
    {
        var carsQuery = db.Cars.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            carsQuery = carsQuery.Where(car =>
                car.Brand.ToLower().Contains(search)
                || car.Model.ToLower().Contains(search)
                || car.Category.ToLower().Contains(search));
        }

        carsQuery = ApplySorting(carsQuery, query);
        var cars = await carsQuery.ToListAsync();

        return cars.Select(car => car.ToResponse()).ToList();
    }

    public async Task<CarResponseDto> GetByIdAsync(int id)
    {
        var car = await db.Cars.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id);
        return car?.ToResponse() ?? throw new NotFoundException("Автомобиль не найден.");
    }

    public async Task<CarResponseDto> CreateAsync(SaveCarRequestDto request)
    {
        Validate(request);
        var car = new Car();
        request.Apply(car);

        db.Cars.Add(car);
        await db.SaveChangesAsync();
        return car.ToResponse();
    }

    public async Task UpdateAsync(int id, SaveCarRequestDto request)
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

    private static void Validate(SaveCarRequestDto request)
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

    private static IQueryable<Car> ApplySorting(IQueryable<Car> cars, CarQueryDto query)
    {
        var isAscending = string.Equals(query.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);

        return query.SortBy?.ToLowerInvariant() switch
        {
            "brand" => isAscending
                ? cars.OrderBy(car => car.Brand).ThenBy(car => car.Model)
                : cars.OrderByDescending(car => car.Brand).ThenByDescending(car => car.Model),
            "dailyrate" => isAscending
                ? cars.OrderBy(car => car.DailyRate)
                : cars.OrderByDescending(car => car.DailyRate),
            "year" => isAscending
                ? cars.OrderBy(car => car.Year)
                : cars.OrderByDescending(car => car.Year),
            _ => cars.OrderByDescending(car => car.Id)
        };
    }
}
