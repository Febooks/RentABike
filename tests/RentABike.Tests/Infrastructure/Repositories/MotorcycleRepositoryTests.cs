using Microsoft.EntityFrameworkCore;
using RentABike.Domain.Entities;
using RentABike.Infrastructure.Data;
using RentABike.Infrastructure.Repositories;

namespace RentABike.Tests.Infrastructure.Repositories;

public class MotorcycleRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly MotorcycleRepository _repository;

    public MotorcycleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new MotorcycleRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ValidMotorcycle_ShouldAdd()
    {
        // Arrange
        var motorcycle = new Motorcycle(2024, "Honda CB 600F", "ABC1234");

        // Act
        var result = await _repository.AddAsync(motorcycle);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(motorcycle.Id);
        var savedMotorcycle = await _context.Motorcycles.FindAsync(motorcycle.Id);
        savedMotorcycle.Should().NotBeNull();
        savedMotorcycle!.LicensePlate.Should().Be("ABC1234");
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ShouldReturnMotorcycle()
    {
        // Arrange
        var motorcycle = new Motorcycle(2024, "Honda CB 600F", "ABC1234");
        await _repository.AddAsync(motorcycle);

        // Act
        var result = await _repository.GetByIdAsync(motorcycle.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(motorcycle.Id);
        result.LicensePlate.Should().Be("ABC1234");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ShouldReturnNull()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByLicensePlateAsync_ExistingPlate_ShouldReturnMotorcycle()
    {
        // Arrange
        var motorcycle = new Motorcycle(2024, "Honda CB 600F", "ABC1234");
        await _repository.AddAsync(motorcycle);

        // Act
        var result = await _repository.GetByLicensePlateAsync("ABC1234");

        // Assert
        result.Should().NotBeNull();
        result!.LicensePlate.Should().Be("ABC1234");
    }

    [Fact]
    public async Task GetByLicensePlateFilterAsync_WithFilter_ShouldReturnFilteredList()
    {
        // Arrange
        var motorcycle1 = new Motorcycle(2024, "Honda CB 600F", "ABC1234");
        var motorcycle2 = new Motorcycle(2024, "Yamaha MT-07", "ABC5678");
        var motorcycle3 = new Motorcycle(2024, "Kawasaki Ninja", "XYZ9999");

        await _repository.AddAsync(motorcycle1);
        await _repository.AddAsync(motorcycle2);
        await _repository.AddAsync(motorcycle3);

        // Act
        var result = await _repository.GetByLicensePlateFilterAsync("ABC");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(m => m.LicensePlate.Contains("ABC")).Should().BeTrue();
    }

    [Fact]
    public async Task GetByLicensePlateFilterAsync_WithoutFilter_ShouldReturnAll()
    {
        // Arrange
        var motorcycle1 = new Motorcycle(2024, "Honda CB 600F", "ABC1234");
        var motorcycle2 = new Motorcycle(2024, "Yamaha MT-07", "ABC5678");

        await _repository.AddAsync(motorcycle1);
        await _repository.AddAsync(motorcycle2);

        // Act
        var result = await _repository.GetByLicensePlateFilterAsync(null);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task LicensePlateExistsAsync_ExistingPlate_ShouldReturnTrue()
    {
        // Arrange
        var motorcycle = new Motorcycle(2024, "Honda CB 600F", "ABC1234");
        await _repository.AddAsync(motorcycle);

        // Act
        var result = await _repository.LicensePlateExistsAsync("ABC1234");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task LicensePlateExistsAsync_NonExistingPlate_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.LicensePlateExistsAsync("XYZ9999");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task LicensePlateExistsAsync_WithExcludeId_ShouldExcludeId()
    {
        // Arrange
        var motorcycle1 = new Motorcycle(2024, "Honda CB 600F", "ABC1234");
        var motorcycle2 = new Motorcycle(2024, "Yamaha MT-07", "ABC5678");

        await _repository.AddAsync(motorcycle1);
        await _repository.AddAsync(motorcycle2);

        // Act
        var result = await _repository.LicensePlateExistsAsync("ABC1234", motorcycle1.Id);

        // Assert
        result.Should().BeFalse(); // Should return false because we're excluding the ID that has this plate
    }

    [Fact]
    public async Task UpdateAsync_ValidMotorcycle_ShouldUpdate()
    {
        // Arrange
        var motorcycle = new Motorcycle(2024, "Honda CB 600F", "ABC1234");
        await _repository.AddAsync(motorcycle);
        motorcycle.UpdateLicensePlate("XYZ5678");

        // Act
        await _repository.UpdateAsync(motorcycle);

        // Assert
        var updated = await _repository.GetByIdAsync(motorcycle.Id);
        updated!.LicensePlate.Should().Be("XYZ5678");
    }

    [Fact]
    public async Task DeleteAsync_ValidMotorcycle_ShouldDelete()
    {
        // Arrange
        var motorcycle = new Motorcycle(2024, "Honda CB 600F", "ABC1234");
        await _repository.AddAsync(motorcycle);

        // Act
        await _repository.DeleteAsync(motorcycle);

        // Assert
        var deleted = await _repository.GetByIdAsync(motorcycle.Id);
        deleted.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

