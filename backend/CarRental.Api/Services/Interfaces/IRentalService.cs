using CarRental.Api.Contracts;

namespace CarRental.Api.Services.Interfaces;

public interface IRentalService
{
    Task<IReadOnlyList<RentalResponse>> GetAllAsync();
    Task<IReadOnlyList<RentalResponse>> GetForUserAsync(string email);
    Task<RentalResponse> CreateAsync(string email, CreateRentalRequest request);
    Task UpdateAsync(int id, UpdateRentalRequest request);
    Task CancelForUserAsync(string email, int id);
    Task DeleteAsync(int id);
}
