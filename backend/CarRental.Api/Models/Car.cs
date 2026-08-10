namespace CarRental.Api.Models;

using System.Text.Json.Serialization;

public class Car
{
    public int Id { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal DailyRate { get; set; }
    public string Transmission { get; set; } = "Автомат";
    public int Seats { get; set; } = 5;
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public string? Description { get; set; }
    [JsonIgnore]
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
