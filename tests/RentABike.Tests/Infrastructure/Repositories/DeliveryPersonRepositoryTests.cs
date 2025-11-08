using Microsoft.EntityFrameworkCore;
using RentABike.Domain.Entities;
using RentABike.Infrastructure.Data;
using RentABike.Infrastructure.Repositories;

namespace RentABike.Tests.Infrastructure.Repositories;

public class DeliveryPersonRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly DeliveryPersonRepository _repository;

    public DeliveryPersonRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new DeliveryPersonRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ValidDeliveryPerson_ShouldAdd()
    {
        // Arrange
        var deliveryPerson = new DeliveryPerson(
            "João Silva",
            "12345678000190",
            new DateTime(1990, 1, 1),
            "12345678901",
            LicenseType.A
        );

        // Act
        var result = await _repository.AddAsync(deliveryPerson);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(deliveryPerson.Id);
        var saved = await _context.DeliveryPersons.FindAsync(deliveryPerson.Id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("João Silva");
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ShouldReturnDeliveryPerson()
    {
        // Arrange
        var deliveryPerson = new DeliveryPerson(
            "João Silva",
            "12345678000190",
            new DateTime(1990, 1, 1),
            "12345678901",
            LicenseType.A
        );
        await _repository.AddAsync(deliveryPerson);

        // Act
        var result = await _repository.GetByIdAsync(deliveryPerson.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(deliveryPerson.Id);
        result.Name.Should().Be("João Silva");
    }

    [Fact]
    public async Task GetByTaxIdNumberAsync_ExistingTaxId_ShouldReturnDeliveryPerson()
    {
        // Arrange
        var deliveryPerson = new DeliveryPerson(
            "João Silva",
            "12345678000190",
            new DateTime(1990, 1, 1),
            "12345678901",
            LicenseType.A
        );
        await _repository.AddAsync(deliveryPerson);

        // Act
        var result = await _repository.GetByTaxIdNumberAsync("12345678000190");

        // Assert
        result.Should().NotBeNull();
        result!.TaxIdNumber.Should().Be("12345678000190");
    }

    [Fact]
    public async Task GetByLicenseNumberAsync_ExistingLicense_ShouldReturnDeliveryPerson()
    {
        // Arrange
        var deliveryPerson = new DeliveryPerson(
            "João Silva",
            "12345678000190",
            new DateTime(1990, 1, 1),
            "12345678901",
            LicenseType.A
        );
        await _repository.AddAsync(deliveryPerson);

        // Act
        var result = await _repository.GetByLicenseNumberAsync("12345678901");

        // Assert
        result.Should().NotBeNull();
        result!.LicenseNumber.Should().Be("12345678901");
    }

    [Fact]
    public async Task TaxIdNumberExistsAsync_ExistingTaxId_ShouldReturnTrue()
    {
        // Arrange
        var deliveryPerson = new DeliveryPerson(
            "João Silva",
            "12345678000190",
            new DateTime(1990, 1, 1),
            "12345678901",
            LicenseType.A
        );
        await _repository.AddAsync(deliveryPerson);

        // Act
        var result = await _repository.TaxIdNumberExistsAsync("12345678000190");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task LicenseNumberExistsAsync_ExistingLicense_ShouldReturnTrue()
    {
        // Arrange
        var deliveryPerson = new DeliveryPerson(
            "João Silva",
            "12345678000190",
            new DateTime(1990, 1, 1),
            "12345678901",
            LicenseType.A
        );
        await _repository.AddAsync(deliveryPerson);

        // Act
        var result = await _repository.LicenseNumberExistsAsync("12345678901");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_ValidDeliveryPerson_ShouldUpdate()
    {
        // Arrange
        var deliveryPerson = new DeliveryPerson(
            "João Silva",
            "12345678000190",
            new DateTime(1990, 1, 1),
            "12345678901",
            LicenseType.A
        );
        await _repository.AddAsync(deliveryPerson);
        deliveryPerson.UpdateLicenseImage("https://example.com/image.png");

        // Act
        await _repository.UpdateAsync(deliveryPerson);

        // Assert
        var updated = await _repository.GetByIdAsync(deliveryPerson.Id);
        updated!.LicenseImageUrl.Should().Be("https://example.com/image.png");
    }

    [Fact]
    public async Task DeleteAsync_ValidDeliveryPerson_ShouldDelete()
    {
        // Arrange
        var deliveryPerson = new DeliveryPerson(
            "João Silva",
            "12345678000190",
            new DateTime(1990, 1, 1),
            "12345678901",
            LicenseType.A
        );
        await _repository.AddAsync(deliveryPerson);

        // Act
        await _repository.DeleteAsync(deliveryPerson);

        // Assert
        var deleted = await _repository.GetByIdAsync(deliveryPerson.Id);
        deleted.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

