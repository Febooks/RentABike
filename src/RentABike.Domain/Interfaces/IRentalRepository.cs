using RentABike.Domain.Entities;

namespace RentABike.Domain.Interfaces;

public interface IRentalRepository : IRepository<Rental>
{
    Task<IEnumerable<Rental>> GetByMotorcycleIdAsync(Guid motorcycleId);
    Task<IEnumerable<Rental>> GetByDeliveryPersonIdAsync(Guid deliveryPersonId);
    Task<Rental?> GetActiveByDeliveryPersonIdAsync(Guid deliveryPersonId);
    Task<bool> HasRentalsByMotorcycleIdAsync(Guid motorcycleId);
}

