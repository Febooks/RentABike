using Microsoft.EntityFrameworkCore;
using RentABike.Domain.Entities;
using RentABike.Domain.Interfaces;
using RentABike.Infrastructure.Data;

namespace RentABike.Infrastructure.Repositories;

public class MotorcycleNotificationRepository : IMotorcycleNotificationRepository
{
    private readonly ApplicationDbContext _context;

    public MotorcycleNotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MotorcycleNotification?> GetByIdAsync(Guid id)
    {
        return await _context.MotorcycleNotifications.FindAsync(id);
    }

    public async Task<IEnumerable<MotorcycleNotification>> GetAllAsync()
    {
        return await _context.MotorcycleNotifications.ToListAsync();
    }

    public async Task<MotorcycleNotification> AddAsync(MotorcycleNotification entity)
    {
        await _context.MotorcycleNotifications.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(MotorcycleNotification entity)
    {
        _context.MotorcycleNotifications.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(MotorcycleNotification entity)
    {
        _context.MotorcycleNotifications.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<MotorcycleNotification, bool>> predicate)
    {
        return await _context.MotorcycleNotifications.AnyAsync(predicate);
    }
}

