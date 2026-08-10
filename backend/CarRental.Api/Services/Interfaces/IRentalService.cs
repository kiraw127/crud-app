using CarRental.Api.Dtos;

namespace CarRental.Api.Services.Interfaces;

public interface IRentalService
{
    Task<IReadOnlyList<RentalResponseDto>> GetAllAsync();
    Task<IReadOnlyList<RentalResponseDto>> GetForUserAsync(string email);
    Task<RentalResponseDto> CreateAsync(string email, CreateRentalRequestDto request);
    Task UpdateAsync(int id, UpdateRentalRequestDto request);
    Task CancelForUserAsync(string email, int id);
    Task DeleteAsync(int id);
}
