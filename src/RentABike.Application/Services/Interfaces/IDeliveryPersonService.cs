using RentABike.Application.DTOs;

namespace RentABike.Application.Services.Interfaces;

public interface IDeliveryPersonService
{
    Task<DeliveryPersonDTO> CreateDeliveryPersonAsync(CreateDeliveryPersonDTO dto);
    Task<DeliveryPersonDTO?> GetDeliveryPersonByIdAsync(Guid id);
    Task<DeliveryPersonDTO> UpdateLicenseImageAsync(Guid id, Stream file, string fileName, string contentType);
}

