using System.ComponentModel.DataAnnotations;

namespace CarRental.Api.Dtos;

public record CarResponseDto(
    int Id,
    string Brand,
    string Model,
    int Year,
    string Category,
    decimal DailyRate,
    string Transmission,
    int Seats,
    string ImageUrl,
    bool IsAvailable,
    string? Description);

public record SaveCarRequestDto(
    [Required] string Brand,
    [Required] string Model,
    [Range(1900, 2100)] int Year,
    [Required] string Category,
    [Range(1, double.MaxValue)] decimal DailyRate,
    [Required] string Transmission,
    [Range(1, 20)] int Seats,
    [Required, Url] string ImageUrl,
    string? Description);

public record CarQueryDto(
    string? Search,
    string? SortBy = null,
    string? SortDirection = null);
