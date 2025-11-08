using RentABike.Application.DTOs;

namespace RentABike.Application.Services.Interfaces;

public interface IMotorcycleService
{
    Task<MotorcycleDTO> CreateMotorcycleAsync(CreateMotorcycleDTO dto);
    Task<IEnumerable<MotorcycleDTO>> ListMotorcyclesAsync(string? licensePlate = null);
    Task<MotorcycleDTO?> GetMotorcycleByIdAsync(Guid id);
    Task<MotorcycleDTO> UpdateLicensePlateAsync(Guid id, UpdateMotorcycleLicensePlateDTO dto);
    Task RemoveMotorcycleAsync(Guid id);
}

