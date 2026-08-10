using System.ComponentModel.DataAnnotations;

namespace CarRental.Api.Contracts;

public record RentalResponse(
    int Id,
    int CarId,
    CarResponse? Car,
    string CustomerName,
    string Phone,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal TotalPrice,
    string Status);

public record CreateRentalRequest(
    [Range(1, int.MaxValue)] int CarId,
    [Required] string Phone,
    DateOnly StartDate,
    DateOnly EndDate);

public record UpdateRentalRequest(
    [Required] string Phone,
    DateOnly StartDate,
    DateOnly EndDate,
    [Required] string Status);
