using System.ComponentModel.DataAnnotations;

namespace CarRental.Api.Dtos;

public record RentalResponseDto(
    int Id,
    int CarId,
    CarResponseDto? Car,
    string CustomerName,
    string Phone,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal TotalPrice,
    string Status);

public record CreateRentalRequestDto(
    [Range(1, int.MaxValue)] int CarId,
    [Required] string Phone,
    DateOnly StartDate,
    DateOnly EndDate);

public record UpdateRentalRequestDto(
    [Required] string Phone,
    DateOnly StartDate,
    DateOnly EndDate,
    [Required] string Status);
