using CarRental.Api.Contracts;

namespace CarRental.Api.Services.Interfaces;

public interface ICarService
{
    Task<IReadOnlyList<CarResponse>> GetAllAsync();
    Task<CarResponse> GetByIdAsync(int id);
    Task<CarResponse> CreateAsync(SaveCarRequest request);
    Task UpdateAsync(int id, SaveCarRequest request);
    Task DeleteAsync(int id);
}
