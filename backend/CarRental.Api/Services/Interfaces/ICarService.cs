using CarRental.Api.Dtos;

namespace CarRental.Api.Services.Interfaces;

public interface ICarService
{
    Task<IReadOnlyList<CarResponseDto>> GetAllAsync(CarQueryDto query);
    Task<CarResponseDto> GetByIdAsync(int id);
    Task<CarResponseDto> CreateAsync(SaveCarRequestDto request);
    Task UpdateAsync(int id, SaveCarRequestDto request);
    Task DeleteAsync(int id);
}
