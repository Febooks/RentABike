using Microsoft.EntityFrameworkCore;
using RentABike.Domain.Entities;
using RentABike.Domain.Interfaces;
using RentABike.Infrastructure.Data;

namespace RentABike.Infrastructure.Repositories;

public class DeliveryPersonRepository : IDeliveryPersonRepository
{
    private readonly ApplicationDbContext _context;

    public DeliveryPersonRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DeliveryPerson?> GetByIdAsync(Guid id)
    {
        return await _context.DeliveryPersons.FindAsync(id);
    }

    public async Task<IEnumerable<DeliveryPerson>> GetAllAsync()
    {
        return await _context.DeliveryPersons.ToListAsync();
    }

    public async Task<DeliveryPerson> AddAsync(DeliveryPerson entity)
    {
        await _context.DeliveryPersons.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(DeliveryPerson entity)
    {
        _context.DeliveryPersons.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(DeliveryPerson entity)
    {
        _context.DeliveryPersons.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<DeliveryPerson, bool>> predicate)
    {
        return await _context.DeliveryPersons.AnyAsync(predicate);
    }

    public async Task<DeliveryPerson?> GetByTaxIdNumberAsync(string taxIdNumber)
    {
        return await _context.DeliveryPersons.FirstOrDefaultAsync(e => e.TaxIdNumber == taxIdNumber);
    }

    public async Task<DeliveryPerson?> GetByLicenseNumberAsync(string licenseNumber)
    {
        return await _context.DeliveryPersons.FirstOrDefaultAsync(e => e.LicenseNumber == licenseNumber);
    }

    public async Task<bool> TaxIdNumberExistsAsync(string taxIdNumber, Guid? excludeId = null)
    {
        var query = _context.DeliveryPersons.Where(e => e.TaxIdNumber == taxIdNumber);

        if (excludeId.HasValue)
        {
            query = query.Where(e => e.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> LicenseNumberExistsAsync(string licenseNumber, Guid? excludeId = null)
    {
        var query = _context.DeliveryPersons.Where(e => e.LicenseNumber == licenseNumber);

        if (excludeId.HasValue)
        {
            query = query.Where(e => e.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}

