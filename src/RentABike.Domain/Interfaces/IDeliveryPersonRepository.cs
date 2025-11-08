using RentABike.Domain.Entities;

namespace RentABike.Domain.Interfaces;

public interface IDeliveryPersonRepository : IRepository<DeliveryPerson>
{
    Task<DeliveryPerson?> GetByTaxIdNumberAsync(string taxIdNumber);
    Task<DeliveryPerson?> GetByLicenseNumberAsync(string licenseNumber);
    Task<bool> TaxIdNumberExistsAsync(string taxIdNumber, Guid? excludeId = null);
    Task<bool> LicenseNumberExistsAsync(string licenseNumber, Guid? excludeId = null);
}

