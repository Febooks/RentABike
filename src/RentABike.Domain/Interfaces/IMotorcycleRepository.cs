using RentABike.Domain.Entities;

namespace RentABike.Domain.Interfaces;

public interface IMotorcycleRepository : IRepository<Motorcycle>
{
    Task<Motorcycle?> GetByLicensePlateAsync(string licensePlate);
    Task<IEnumerable<Motorcycle>> GetByLicensePlateFilterAsync(string? licensePlate);
    Task<bool> LicensePlateExistsAsync(string licensePlate, Guid? excludeId = null);
}

