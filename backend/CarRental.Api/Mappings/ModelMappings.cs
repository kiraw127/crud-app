using CarRental.Api.Contracts;
using CarRental.Api.Models;

namespace CarRental.Api.Mappings;

public static class ModelMappings
{
    public static CarResponse ToResponse(this Car car) => new(
        car.Id,
        car.Brand,
        car.Model,
        car.Year,
        car.Category,
        car.DailyRate,
        car.Transmission,
        car.Seats,
        car.ImageUrl,
        car.IsAvailable,
        car.Description);

    public static RentalResponse ToResponse(this Rental rental) => new(
        rental.Id,
        rental.CarId,
        rental.Car?.ToResponse(),
        rental.CustomerName,
        rental.Phone,
        rental.StartDate,
        rental.EndDate,
        rental.TotalPrice,
        rental.Status);

    public static void Apply(this SaveCarRequest request, Car car)
    {
        car.Brand = request.Brand.Trim();
        car.Model = request.Model.Trim();
        car.Year = request.Year;
        car.Category = request.Category.Trim();
        car.DailyRate = request.DailyRate;
        car.Transmission = request.Transmission.Trim();
        car.Seats = request.Seats;
        car.ImageUrl = request.ImageUrl.Trim();
        car.Description = request.Description?.Trim();
    }
}
