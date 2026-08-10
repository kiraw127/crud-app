namespace CarRental.Api.Models;

public class Rental
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public Car? Car { get; set; }
    public int? UserId { get; set; }
    public AppUser? User { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Активна";
}
