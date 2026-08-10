namespace CarRental.Api.Models;

using System.Text.Json.Serialization;

public class AppUser
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Customer";
    [JsonIgnore]
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
