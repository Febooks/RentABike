using AutoMapper;
using RentABike.Application.DTOs;
using RentABike.Application.Services;
using RentABike.Application.Services.Interfaces;
using RentABike.Domain.Entities;
using RentABike.Domain.Interfaces;

namespace RentABike.Tests.Application.Services;

public class RentalServiceTests
{
    private readonly Mock<IRentalRepository> _rentalRepositoryMock;
    private readonly Mock<IMotorcycleRepository> _motorcycleRepositoryMock;
    private readonly Mock<IDeliveryPersonRepository> _deliveryPersonRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly RentalService _service;

    public RentalServiceTests()
    {
        _rentalRepositoryMock = new Mock<IRentalRepository>();
        _motorcycleRepositoryMock = new Mock<IMotorcycleRepository>();
        _deliveryPersonRepositoryMock = new Mock<IDeliveryPersonRepository>();
        _mapperMock = new Mock<IMapper>();

        _service = new RentalService(
            _rentalRepositoryMock.Object,
            _motorcycleRepositoryMock.Object,
            _deliveryPersonRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task CreateRentalAsync_ValidData_ShouldCreate()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var dto = new CreateRentalDTO
        {
            MotorcycleId = motorcycleId,
            DeliveryPersonId = deliveryPersonId,
            PlanDays = 7
        };

        var motorcycle = new Motorcycle(2024, "Honda CB 600F", "ABC1234");
        var deliveryPerson = new DeliveryPerson(
            "João Silva",
            "12345678000190",
            new DateTime(1990, 1, 1),
            "12345678901",
            LicenseType.A
        );

        var rental = new Rental(
            motorcycleId,
            deliveryPersonId,
            DateTime.UtcNow.Date.AddDays(1),
            DateTime.UtcNow.Date.AddDays(8),
            7
        );

        var rentalDto = new RentalDTO
        {
            Id = rental.Id,
            MotorcycleId = motorcycleId,
            DeliveryPersonId = deliveryPersonId,
            PlanDays = 7
        };

        _motorcycleRepositoryMock
            .Setup(x => x.GetByIdAsync(motorcycleId))
            .ReturnsAsync(motorcycle);

        _deliveryPersonRepositoryMock
            .Setup(x => x.GetByIdAsync(deliveryPersonId))
            .ReturnsAsync(deliveryPerson);

        _rentalRepositoryMock
            .Setup(x => x.GetActiveByDeliveryPersonIdAsync(deliveryPersonId))
            .ReturnsAsync((Rental?)null);

        _rentalRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Rental>()))
            .ReturnsAsync(rental);

        _mapperMock
            .Setup(x => x.Map<RentalDTO>(It.IsAny<Rental>()))
            .Returns(rentalDto);

        // Act
        var result = await _service.CreateRentalAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.MotorcycleId.Should().Be(motorcycleId);
        result.DeliveryPersonId.Should().Be(deliveryPersonId);
        _rentalRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Rental>()), Times.Once);
    }

    [Fact]
    public async Task CreateRentalAsync_NonExistingMotorcycle_ShouldThrowException()
    {
        // Arrange
        var dto = new CreateRentalDTO
        {
            MotorcycleId = Guid.NewGuid(),
            DeliveryPersonId = Guid.NewGuid(),
            PlanDays = 7
        };

        _motorcycleRepositoryMock
            .Setup(x => x.GetByIdAsync(dto.MotorcycleId))
            .ReturnsAsync((Motorcycle?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateRentalAsync(dto));
    }

    [Fact]
    public async Task CreateRentalAsync_DeliveryPersonCannotRent_ShouldThrowException()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var dto = new CreateRentalDTO
        {
            MotorcycleId = motorcycleId,
            DeliveryPersonId = deliveryPersonId,
            PlanDays = 7
        };

        var motorcycle = new Motorcycle(2024, "Honda CB 600F", "ABC1234");
        var deliveryPerson = new DeliveryPerson(
            "João Silva",
            "12345678000190",
            new DateTime(1990, 1, 1),
            "12345678901",
            LicenseType.B // Cannot rent
        );

        _motorcycleRepositoryMock
            .Setup(x => x.GetByIdAsync(motorcycleId))
            .ReturnsAsync(motorcycle);

        _deliveryPersonRepositoryMock
            .Setup(x => x.GetByIdAsync(deliveryPersonId))
            .ReturnsAsync(deliveryPerson);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateRentalAsync(dto));
    }

    [Fact]
    public async Task CreateRentalAsync_ActiveRentalExists_ShouldThrowException()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var dto = new CreateRentalDTO
        {
            MotorcycleId = motorcycleId,
            DeliveryPersonId = deliveryPersonId,
            PlanDays = 7
        };

        var motorcycle = new Motorcycle(2024, "Honda CB 600F", "ABC1234");
        var deliveryPerson = new DeliveryPerson(
            "João Silva",
            "12345678000190",
            new DateTime(1990, 1, 1),
            "12345678901",
            LicenseType.A
        );

        var activeRental = new Rental(
            Guid.NewGuid(),
            deliveryPersonId,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(7),
            7
        );

        _motorcycleRepositoryMock
            .Setup(x => x.GetByIdAsync(motorcycleId))
            .ReturnsAsync(motorcycle);

        _deliveryPersonRepositoryMock
            .Setup(x => x.GetByIdAsync(deliveryPersonId))
            .ReturnsAsync(deliveryPerson);

        _rentalRepositoryMock
            .Setup(x => x.GetActiveByDeliveryPersonIdAsync(deliveryPersonId))
            .ReturnsAsync(activeRental);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateRentalAsync(dto));
    }

    [Fact]
    public async Task ReturnRentalAsync_ValidData_ShouldUpdate()
    {
        // Arrange
        var id = Guid.NewGuid();
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expectedEndDate = new DateTime(2024, 1, 8, 0, 0, 0, DateTimeKind.Utc);

        var rental = new Rental(motorcycleId, deliveryPersonId, startDate, expectedEndDate, 7);
        var returnDate = new DateTime(2024, 1, 5, 0, 0, 0, DateTimeKind.Utc);

        var dto = new ReturnRentalDTO { ReturnDate = returnDate };
        var rentalDto = new RentalDTO { Id = id };

        _rentalRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(rental);

        _rentalRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Rental>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(x => x.Map<RentalDTO>(It.IsAny<Rental>()))
            .Returns(rentalDto);

        // Act
        var result = await _service.ReturnRentalAsync(id, dto);

        // Assert
        result.Should().NotBeNull();
        _rentalRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Rental>()), Times.Once);
    }

    [Fact]
    public async Task ReturnRentalAsync_NonExistingRental_ShouldThrowException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new ReturnRentalDTO { ReturnDate = DateTime.UtcNow };

        _rentalRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync((Rental?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.ReturnRentalAsync(id, dto));
    }

    [Fact]
    public async Task CalculateRentalAsync_ValidData_ShouldCalculate()
    {
        // Arrange
        var id = Guid.NewGuid();
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expectedEndDate = new DateTime(2024, 1, 8, 0, 0, 0, DateTimeKind.Utc);

        var rental = new Rental(motorcycleId, deliveryPersonId, startDate, expectedEndDate, 7);
        var returnDate = new DateTime(2024, 1, 5, 0, 0, 0, DateTimeKind.Utc);

        var dto = new CalculateRentalDTO { ReturnDate = returnDate };
        var rentalDto = new RentalDTO { Id = Guid.NewGuid() }; // Will be updated by service

        _rentalRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(rental);

        _mapperMock
            .Setup(x => x.Map<RentalDTO>(It.IsAny<Rental>()))
            .Returns(() => new RentalDTO { Id = Guid.NewGuid() }); // Returns new DTO each time

        // Act
        var result = await _service.CalculateRentalAsync(id, dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(rental.Id); // The ID is preserved from the original rental
        _rentalRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Rental>()), Times.Never);
    }
}

