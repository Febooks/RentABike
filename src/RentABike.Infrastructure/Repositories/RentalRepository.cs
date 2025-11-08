using Microsoft.EntityFrameworkCore;
using RentABike.Domain.Entities;
using RentABike.Domain.Interfaces;
using RentABike.Infrastructure.Data;

namespace RentABike.Infrastructure.Repositories;

public class RentalRepository : IRentalRepository
{
    private readonly ApplicationDbContext _context;

    public RentalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Rental?> GetByIdAsync(Guid id)
    {
        return await _context.Rentals.FindAsync(id);
    }

    public async Task<IEnumerable<Rental>> GetAllAsync()
    {
        return await _context.Rentals.ToListAsync();
    }

    public async Task<Rental> AddAsync(Rental entity)
    {
        await _context.Rentals.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Rental entity)
    {
        _context.Rentals.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Rental entity)
    {
        _context.Rentals.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<Rental, bool>> predicate)
    {
        return await _context.Rentals.AnyAsync(predicate);
    }

    public async Task<IEnumerable<Rental>> GetByMotorcycleIdAsync(Guid motorcycleId)
    {
        return await _context.Rentals
            .Where(l => l.MotorcycleId == motorcycleId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetByDeliveryPersonIdAsync(Guid deliveryPersonId)
    {
        return await _context.Rentals
            .Where(l => l.DeliveryPersonId == deliveryPersonId)
            .ToListAsync();
    }

    public async Task<Rental?> GetActiveByDeliveryPersonIdAsync(Guid deliveryPersonId)
    {
        return await _context.Rentals
            .Where(l => l.DeliveryPersonId == deliveryPersonId && l.ReturnDate == null)
            .OrderByDescending(l => l.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> HasRentalsByMotorcycleIdAsync(Guid motorcycleId)
    {
        return await _context.Rentals.AnyAsync(l => l.MotorcycleId == motorcycleId);
    }
}

