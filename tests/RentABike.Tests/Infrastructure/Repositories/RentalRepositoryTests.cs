using Microsoft.EntityFrameworkCore;
using RentABike.Domain.Entities;
using RentABike.Infrastructure.Data;
using RentABike.Infrastructure.Repositories;

namespace RentABike.Tests.Infrastructure.Repositories;

public class RentalRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly RentalRepository _repository;

    public RentalRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new RentalRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ValidRental_ShouldAdd()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expectedEndDate = new DateTime(2024, 1, 8, 0, 0, 0, DateTimeKind.Utc);

        var rental = new Rental(motorcycleId, deliveryPersonId, startDate, expectedEndDate, 7);

        // Act
        var result = await _repository.AddAsync(rental);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(rental.Id);
        var saved = await _context.Rentals.FindAsync(rental.Id);
        saved.Should().NotBeNull();
        saved!.PlanDays.Should().Be(7);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ShouldReturnRental()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expectedEndDate = new DateTime(2024, 1, 8, 0, 0, 0, DateTimeKind.Utc);

        var rental = new Rental(motorcycleId, deliveryPersonId, startDate, expectedEndDate, 7);
        await _repository.AddAsync(rental);

        // Act
        var result = await _repository.GetByIdAsync(rental.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(rental.Id);
        result.PlanDays.Should().Be(7);
    }

    [Fact]
    public async Task GetByMotorcycleIdAsync_ExistingMotorcycle_ShouldReturnRentals()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expectedEndDate = new DateTime(2024, 1, 8, 0, 0, 0, DateTimeKind.Utc);

        var rental1 = new Rental(motorcycleId, deliveryPersonId, startDate, expectedEndDate, 7);
        var rental2 = new Rental(motorcycleId, deliveryPersonId, startDate.AddDays(10), expectedEndDate.AddDays(10), 7);

        await _repository.AddAsync(rental1);
        await _repository.AddAsync(rental2);

        // Act
        var result = await _repository.GetByMotorcycleIdAsync(motorcycleId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(r => r.MotorcycleId == motorcycleId).Should().BeTrue();
    }

    [Fact]
    public async Task GetByDeliveryPersonIdAsync_ExistingDeliveryPerson_ShouldReturnRentals()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expectedEndDate = new DateTime(2024, 1, 8, 0, 0, 0, DateTimeKind.Utc);

        var rental1 = new Rental(motorcycleId, deliveryPersonId, startDate, expectedEndDate, 7);
        var rental2 = new Rental(motorcycleId, deliveryPersonId, startDate.AddDays(10), expectedEndDate.AddDays(10), 7);

        await _repository.AddAsync(rental1);
        await _repository.AddAsync(rental2);

        // Act
        var result = await _repository.GetByDeliveryPersonIdAsync(deliveryPersonId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(r => r.DeliveryPersonId == deliveryPersonId).Should().BeTrue();
    }

    [Fact]
    public async Task GetActiveByDeliveryPersonIdAsync_ActiveRental_ShouldReturnRental()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expectedEndDate = new DateTime(2024, 1, 8, 0, 0, 0, DateTimeKind.Utc);

        var activeRental = new Rental(motorcycleId, deliveryPersonId, startDate, expectedEndDate, 7);
        await _repository.AddAsync(activeRental);

        // Act
        var result = await _repository.GetActiveByDeliveryPersonIdAsync(deliveryPersonId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(activeRental.Id);
        result.ReturnDate.Should().BeNull();
    }

    [Fact]
    public async Task GetActiveByDeliveryPersonIdAsync_NoActiveRental_ShouldReturnNull()
    {
        // Arrange
        var deliveryPersonId = Guid.NewGuid();

        // Act
        var result = await _repository.GetActiveByDeliveryPersonIdAsync(deliveryPersonId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task HasRentalsByMotorcycleIdAsync_WithRentals_ShouldReturnTrue()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expectedEndDate = new DateTime(2024, 1, 8, 0, 0, 0, DateTimeKind.Utc);

        var rental = new Rental(motorcycleId, deliveryPersonId, startDate, expectedEndDate, 7);
        await _repository.AddAsync(rental);

        // Act
        var result = await _repository.HasRentalsByMotorcycleIdAsync(motorcycleId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasRentalsByMotorcycleIdAsync_NoRentals_ShouldReturnFalse()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();

        // Act
        var result = await _repository.HasRentalsByMotorcycleIdAsync(motorcycleId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_ValidRental_ShouldUpdate()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expectedEndDate = new DateTime(2024, 1, 8, 0, 0, 0, DateTimeKind.Utc);

        var rental = new Rental(motorcycleId, deliveryPersonId, startDate, expectedEndDate, 7);
        await _repository.AddAsync(rental);

        var returnDate = new DateTime(2024, 1, 5, 0, 0, 0, DateTimeKind.Utc);
        rental.RegisterReturn(returnDate);

        // Act
        await _repository.UpdateAsync(rental);

        // Assert
        var updated = await _repository.GetByIdAsync(rental.Id);
        updated!.ReturnDate.Should().NotBeNull();
        updated.ReturnDate.Should().Be(returnDate);
    }

    [Fact]
    public async Task DeleteAsync_ValidRental_ShouldDelete()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expectedEndDate = new DateTime(2024, 1, 8, 0, 0, 0, DateTimeKind.Utc);

        var rental = new Rental(motorcycleId, deliveryPersonId, startDate, expectedEndDate, 7);
        await _repository.AddAsync(rental);

        // Act
        await _repository.DeleteAsync(rental);

        // Assert
        var deleted = await _repository.GetByIdAsync(rental.Id);
        deleted.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

