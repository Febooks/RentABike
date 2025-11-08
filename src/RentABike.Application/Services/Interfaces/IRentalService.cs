using RentABike.Application.DTOs;

namespace RentABike.Application.Services.Interfaces;

public interface IRentalService
{
    Task<RentalDTO> CreateRentalAsync(CreateRentalDTO dto);
    Task<RentalDTO?> GetRentalByIdAsync(Guid id);
    Task<RentalDTO> ReturnRentalAsync(Guid id, ReturnRentalDTO dto);
    Task<RentalDTO> CalculateRentalAsync(Guid id, CalculateRentalDTO dto);
}

