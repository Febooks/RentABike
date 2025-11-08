using Microsoft.EntityFrameworkCore;
using RentABike.Domain.Entities;
using RentABike.Domain.Interfaces;
using RentABike.Infrastructure.Data;

namespace RentABike.Infrastructure.Repositories;

public class MotorcycleRepository : IMotorcycleRepository
{
    private readonly ApplicationDbContext _context;

    public MotorcycleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Motorcycle?> GetByIdAsync(Guid id)
    {
        return await _context.Motorcycles.FindAsync(id);
    }

    public async Task<IEnumerable<Motorcycle>> GetAllAsync()
    {
        return await _context.Motorcycles.ToListAsync();
    }

    public async Task<Motorcycle> AddAsync(Motorcycle entity)
    {
        await _context.Motorcycles.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Motorcycle entity)
    {
        _context.Motorcycles.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Motorcycle entity)
    {
        _context.Motorcycles.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<Motorcycle, bool>> predicate)
    {
        return await _context.Motorcycles.AnyAsync(predicate);
    }

    public async Task<Motorcycle?> GetByLicensePlateAsync(string licensePlate)
    {
        return await _context.Motorcycles.FirstOrDefaultAsync(m => m.LicensePlate == licensePlate);
    }

    public async Task<IEnumerable<Motorcycle>> GetByLicensePlateFilterAsync(string? licensePlate)
    {
        var query = _context.Motorcycles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(licensePlate))
        {
            query = query.Where(m => m.LicensePlate.Contains(licensePlate));
        }

        return await query.ToListAsync();
    }

    public async Task<bool> LicensePlateExistsAsync(string licensePlate, Guid? excludeId = null)
    {
        var query = _context.Motorcycles.Where(m => m.LicensePlate == licensePlate);

        if (excludeId.HasValue)
        {
            query = query.Where(m => m.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}

