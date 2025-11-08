using AutoMapper;
using RentABike.Application.DTOs;
using RentABike.Application.Services;
using RentABike.Application.Services.Interfaces;
using RentABike.Domain.Entities;
using RentABike.Domain.Events;
using RentABike.Domain.Interfaces;

namespace RentABike.Tests.Application.Services;

public class MotorcycleServiceTests
{
    private readonly Mock<IMotorcycleRepository> _motorcycleRepositoryMock;
    private readonly Mock<IRentalRepository> _rentalRepositoryMock;
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly MotorcycleService _service;

    public MotorcycleServiceTests()
    {
        _motorcycleRepositoryMock = new Mock<IMotorcycleRepository>();
        _rentalRepositoryMock = new Mock<IRentalRepository>();
        _messageBusMock = new Mock<IMessageBus>();
        _mapperMock = new Mock<IMapper>();

        _service = new MotorcycleService(
            _motorcycleRepositoryMock.Object,
            _rentalRepositoryMock.Object,
            _messageBusMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task CreateMotorcycleAsync_ValidData_ShouldCreateAndPublishEvent()
    {
        // Arrange
        var dto = new CreateMotorcycleDTO
        {
            Year = 2024,
            Model = "Honda CB 600F",
            LicensePlate = "ABC1234"
        };

        var motorcycle = new Motorcycle(dto.Year, dto.Model, dto.LicensePlate);
        var motorcycleDto = new MotorcycleDTO
        {
            Id = motorcycle.Id,
            Year = motorcycle.Year,
            Model = motorcycle.Model,
            LicensePlate = motorcycle.LicensePlate
        };

        _motorcycleRepositoryMock
            .Setup(x => x.LicensePlateExistsAsync(dto.LicensePlate, null))
            .ReturnsAsync(false);

        _motorcycleRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Motorcycle>()))
            .ReturnsAsync(motorcycle);

        _mapperMock
            .Setup(x => x.Map<MotorcycleDTO>(It.IsAny<Motorcycle>()))
            .Returns(motorcycleDto);

        // Act
        var result = await _service.CreateMotorcycleAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.LicensePlate.Should().Be(dto.LicensePlate);
        _motorcycleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Motorcycle>()), Times.Once);
        _messageBusMock.Verify(x => x.PublishAsync(It.IsAny<MotorcycleRegisteredEvent>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateMotorcycleAsync_DuplicateLicensePlate_ShouldThrowException()
    {
        // Arrange
        var dto = new CreateMotorcycleDTO
        {
            Year = 2024,
            Model = "Honda CB 600F",
            LicensePlate = "ABC1234"
        };

        _motorcycleRepositoryMock
            .Setup(x => x.LicensePlateExistsAsync(dto.LicensePlate, null))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateMotorcycleAsync(dto));
        _motorcycleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Motorcycle>()), Times.Never);
    }

    [Fact]
    public async Task ListMotorcyclesAsync_WithFilter_ShouldReturnFilteredList()
    {
        // Arrange
        var licensePlate = "ABC";
        var motorcycles = new List<Motorcycle>
        {
            new Motorcycle(2024, "Honda CB 600F", "ABC1234"),
            new Motorcycle(2024, "Yamaha MT-07", "ABC5678")
        };

        var motorcycleDtos = motorcycles.Select(m => new MotorcycleDTO
        {
            Id = m.Id,
            Year = m.Year,
            Model = m.Model,
            LicensePlate = m.LicensePlate
        }).ToList();

        _motorcycleRepositoryMock
            .Setup(x => x.GetByLicensePlateFilterAsync(licensePlate))
            .ReturnsAsync(motorcycles);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<MotorcycleDTO>>(motorcycles))
            .Returns(motorcycleDtos);

        // Act
        var result = await _service.ListMotorcyclesAsync(licensePlate);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetMotorcycleByIdAsync_ExistingId_ShouldReturnMotorcycle()
    {
        // Arrange
        var id = Guid.NewGuid();
        var motorcycle = new Motorcycle(2024, "Honda CB 600F", "ABC1234");
        var motorcycleDto = new MotorcycleDTO
        {
            Id = motorcycle.Id,
            Year = motorcycle.Year,
            Model = motorcycle.Model,
            LicensePlate = motorcycle.LicensePlate
        };

        _motorcycleRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(motorcycle);

        _mapperMock
            .Setup(x => x.Map<MotorcycleDTO>(motorcycle))
            .Returns(motorcycleDto);

        // Act
        var result = await _service.GetMotorcycleByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(motorcycle.Id);
    }

    [Fact]
    public async Task GetMotorcycleByIdAsync_NonExistingId_ShouldReturnNull()
    {
        // Arrange
        var id = Guid.NewGuid();

        _motorcycleRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync((Motorcycle?)null);

        // Act
        var result = await _service.GetMotorcycleByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateLicensePlateAsync_ValidData_ShouldUpdate()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UpdateMotorcycleLicensePlateDTO { LicensePlate = "XYZ5678" };
        var motorcycle = new Motorcycle(2024, "Honda CB 600F", "ABC1234");

        _motorcycleRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(motorcycle);

        _motorcycleRepositoryMock
            .Setup(x => x.LicensePlateExistsAsync(dto.LicensePlate, id))
            .ReturnsAsync(false);

        _motorcycleRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Motorcycle>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(x => x.Map<MotorcycleDTO>(It.IsAny<Motorcycle>()))
            .Returns(new MotorcycleDTO { Id = id, LicensePlate = dto.LicensePlate });

        // Act
        var result = await _service.UpdateLicensePlateAsync(id, dto);

        // Assert
        result.Should().NotBeNull();
        result.LicensePlate.Should().Be(dto.LicensePlate);
        _motorcycleRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Motorcycle>()), Times.Once);
    }

    [Fact]
    public async Task UpdateLicensePlateAsync_NonExistingMotorcycle_ShouldThrowException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UpdateMotorcycleLicensePlateDTO { LicensePlate = "XYZ5678" };

        _motorcycleRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync((Motorcycle?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateLicensePlateAsync(id, dto));
    }

    [Fact]
    public async Task RemoveMotorcycleAsync_NoRentals_ShouldRemove()
    {
        // Arrange
        var id = Guid.NewGuid();
        var motorcycle = new Motorcycle(2024, "Honda CB 600F", "ABC1234");

        _motorcycleRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(motorcycle);

        _rentalRepositoryMock
            .Setup(x => x.HasRentalsByMotorcycleIdAsync(id))
            .ReturnsAsync(false);

        _motorcycleRepositoryMock
            .Setup(x => x.DeleteAsync(It.IsAny<Motorcycle>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.RemoveMotorcycleAsync(id);

        // Assert
        _motorcycleRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Motorcycle>()), Times.Once);
    }

    [Fact]
    public async Task RemoveMotorcycleAsync_WithRentals_ShouldThrowException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var motorcycle = new Motorcycle(2024, "Honda CB 600F", "ABC1234");

        _motorcycleRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(motorcycle);

        _rentalRepositoryMock
            .Setup(x => x.HasRentalsByMotorcycleIdAsync(id))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RemoveMotorcycleAsync(id));
        _motorcycleRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Motorcycle>()), Times.Never);
    }
}

