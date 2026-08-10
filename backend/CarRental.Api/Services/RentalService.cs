using CarRental.Api.Dtos;
using CarRental.Api.Data;
using CarRental.Api.Exceptions;
using CarRental.Api.Mappings;
using CarRental.Api.Models;
using CarRental.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Api.Services;

public class RentalService(AppDbContext db) : IRentalService
{
    public async Task<IReadOnlyList<RentalResponseDto>> GetAllAsync()
    {
        var rentals = await RentalQuery().ToListAsync();
        return rentals.Select(rental => rental.ToResponse()).ToList();
    }

    public async Task<IReadOnlyList<RentalResponseDto>> GetForUserAsync(string email)
    {
        var user = await FindUserAsync(email);
        var rentals = await RentalQuery()
            .Where(rental => rental.UserId == user.Id)
            .ToListAsync();

        return rentals.Select(rental => rental.ToResponse()).ToList();
    }

    public async Task<RentalResponseDto> CreateAsync(string email, CreateRentalRequestDto request)
    {
        ValidateDates(request.StartDate, request.EndDate);
        var user = await FindUserAsync(email);

        await using var transaction = await db.Database.BeginTransactionAsync();
        var car = await db.Cars.AsNoTracking().SingleOrDefaultAsync(item => item.Id == request.CarId)
            ?? throw new NotFoundException("Автомобиль не найден.");
        var claimedCars = await db.Cars
            .Where(item => item.Id == request.CarId && item.IsAvailable)
            .ExecuteUpdateAsync(update => update.SetProperty(item => item.IsAvailable, false));
        if (claimedCars == 0)
        {
            throw new ConflictException("Автомобиль уже недоступен.");
        }

        var rentalDays = request.EndDate.DayNumber - request.StartDate.DayNumber;
        var rental = new Rental
        {
            CarId = car.Id,
            UserId = user.Id,
            CustomerName = user.Name,
            Phone = request.Phone.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalPrice = rentalDays * car.DailyRate,
            Status = "Активна"
        };

        db.Rentals.Add(rental);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        rental.Car = car;
        car.IsAvailable = false;
        return rental.ToResponse();
    }

    public async Task UpdateAsync(int id, UpdateRentalRequestDto request)
    {
        ValidateDates(request.StartDate, request.EndDate);
        var rental = await db.Rentals.FindAsync(id)
            ?? throw new NotFoundException("Аренда не найдена.");
        var dailyRate = await db.Cars
            .Where(car => car.Id == rental.CarId)
            .Select(car => car.DailyRate)
            .SingleAsync();

        rental.Phone = request.Phone.Trim();
        rental.StartDate = request.StartDate;
        rental.EndDate = request.EndDate;
        rental.Status = request.Status.Trim();
        rental.TotalPrice = (request.EndDate.DayNumber - request.StartDate.DayNumber) * dailyRate;
        await db.SaveChangesAsync();
    }

    public async Task CancelForUserAsync(string email, int id)
    {
        var user = await FindUserAsync(email);
        var rental = await db.Rentals.SingleOrDefaultAsync(
            item => item.Id == id && item.UserId == user.Id)
            ?? throw new NotFoundException("Аренда не найдена.");

        await DeleteAndReleaseCarAsync(rental);
    }

    public async Task DeleteAsync(int id)
    {
        var rental = await db.Rentals.FindAsync(id)
            ?? throw new NotFoundException("Аренда не найдена.");
        await DeleteAndReleaseCarAsync(rental);
    }

    private IQueryable<Rental> RentalQuery() => db.Rentals
        .AsNoTracking()
        .Include(rental => rental.Car)
        .OrderByDescending(rental => rental.Id);

    private async Task<AppUser> FindUserAsync(string email)
    {
        return await db.Users.SingleOrDefaultAsync(user => user.Email == email)
            ?? throw new UnauthorizedAccessException("Пользователь не найден.");
    }

    private async Task DeleteAndReleaseCarAsync(Rental rental)
    {
        var car = await db.Cars.FindAsync(rental.CarId);
        if (car is not null) car.IsAvailable = true;

        db.Rentals.Remove(rental);
        await db.SaveChangesAsync();
    }

    private static void ValidateDates(DateOnly startDate, DateOnly endDate)
    {
        if (endDate <= startDate)
        {
            throw new RequestValidationException(
                "Дата окончания должна быть позже даты начала.");
        }
    }
}
