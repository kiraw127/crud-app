using CarRental.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Rental> Rentals => Set<Rental>();
    public DbSet<AppUser> Users => Set<AppUser>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Car>().Property(x => x.DailyRate).HasPrecision(12, 2);
        b.Entity<Rental>().Property(x => x.TotalPrice).HasPrecision(12, 2);
        b.Entity<AppUser>().HasIndex(x => x.Email).IsUnique();
        b.Entity<Car>().HasData(
            new Car
            {
                Id = 1,
                Brand = "BMW",
                Model = "X5",
                Year = 2023,
                Category = "SUV",
                DailyRate = 52000,
                Transmission = "Автомат",
                Seats = 5,
                IsAvailable = true,
                ImageUrl = "https://images.unsplash.com/photo-1556189250-72ba954cfc2b?auto=format&fit=crop&w=1200&q=80",
                Description = "Премиальный кроссовер для любых маршрутов"
            },
            new Car
            {
                Id = 2,
                Brand = "Toyota",
                Model = "Camry",
                Year = 2024,
                Category = "Бизнес",
                DailyRate = 30000,
                Transmission = "Автомат",
                Seats = 5,
                IsAvailable = true,
                ImageUrl = "https://images.unsplash.com/photo-1621007947382-bb3c3994e3fb?auto=format&fit=crop&w=1200&q=80",
                Description = "Комфортный седан бизнес-класса"
            },
            new Car
            {
                Id = 3,
                Brand = "Mercedes-Benz",
                Model = "E 200",
                Year = 2022,
                Category = "Премиум",
                DailyRate = 65000,
                Transmission = "Автомат",
                Seats = 5,
                IsAvailable = false,
                ImageUrl = "https://images.unsplash.com/photo-1618843479313-40f8afb4b4d8?auto=format&fit=crop&w=1200&q=80",
                Description = "Статус и безупречный комфорт"
            });
    }
}
