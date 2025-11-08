using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentABike.API.Controllers;
using RentABike.Application.DTOs;
using RentABike.Application.Services.Interfaces;

namespace RentABike.Tests.API.Controllers;

public class MotorcyclesControllerTests
{
    private readonly Mock<IMotorcycleService> _motorcycleServiceMock;
    private readonly MotorcyclesController _controller;

    public MotorcyclesControllerTests()
    {
        _motorcycleServiceMock = new Mock<IMotorcycleService>();
        _controller = new MotorcyclesController(_motorcycleServiceMock.Object);
    }

    [Fact]
    public async Task CreateMotorcycle_ValidData_ShouldReturnCreated()
    {
        // Arrange
        var dto = new CreateMotorcycleDTO
        {
            Year = 2024,
            Model = "Honda CB 600F",
            LicensePlate = "ABC1234"
        };

        var motorcycleDto = new MotorcycleDTO
        {
            Id = Guid.NewGuid(),
            Year = dto.Year,
            Model = dto.Model,
            LicensePlate = dto.LicensePlate
        };

        _motorcycleServiceMock
            .Setup(x => x.CreateMotorcycleAsync(dto))
            .ReturnsAsync(motorcycleDto);

        // Act
        var result = await _controller.CreateMotorcycle(dto);

        // Assert
        result.Should().NotBeNull();
        var createdAtResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtResult.Value.Should().BeEquivalentTo(motorcycleDto);
    }

    [Fact]
    public async Task CreateMotorcycle_InvalidOperationException_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new CreateMotorcycleDTO
        {
            Year = 2024,
            Model = "Honda CB 600F",
            LicensePlate = "ABC1234"
        };

        _motorcycleServiceMock
            .Setup(x => x.CreateMotorcycleAsync(dto))
            .ThrowsAsync(new InvalidOperationException("A placa já está cadastrada no sistema."));

        // Act
        var result = await _controller.CreateMotorcycle(dto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ListMotorcycles_WithFilter_ShouldReturnOk()
    {
        // Arrange
        var licensePlate = "ABC";
        var motorcycles = new List<MotorcycleDTO>
        {
            new MotorcycleDTO { Id = Guid.NewGuid(), LicensePlate = "ABC1234" },
            new MotorcycleDTO { Id = Guid.NewGuid(), LicensePlate = "ABC5678" }
        };

        _motorcycleServiceMock
            .Setup(x => x.ListMotorcyclesAsync(licensePlate))
            .ReturnsAsync(motorcycles);

        // Act
        var result = await _controller.ListMotorcycles(licensePlate);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(motorcycles);
    }

    [Fact]
    public async Task GetMotorcycle_ExistingId_ShouldReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var motorcycleDto = new MotorcycleDTO
        {
            Id = id,
            Year = 2024,
            Model = "Honda CB 600F",
            LicensePlate = "ABC1234"
        };

        _motorcycleServiceMock
            .Setup(x => x.GetMotorcycleByIdAsync(id))
            .ReturnsAsync(motorcycleDto);

        // Act
        var result = await _controller.GetMotorcycle(id);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(motorcycleDto);
    }

    [Fact]
    public async Task GetMotorcycle_NonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        _motorcycleServiceMock
            .Setup(x => x.GetMotorcycleByIdAsync(id))
            .ReturnsAsync((MotorcycleDTO?)null);

        // Act
        var result = await _controller.GetMotorcycle(id);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task UpdateLicensePlate_ValidData_ShouldReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UpdateMotorcycleLicensePlateDTO { LicensePlate = "XYZ5678" };
        var motorcycleDto = new MotorcycleDTO
        {
            Id = id,
            LicensePlate = dto.LicensePlate
        };

        _motorcycleServiceMock
            .Setup(x => x.UpdateLicensePlateAsync(id, dto))
            .ReturnsAsync(motorcycleDto);

        // Act
        var result = await _controller.UpdateLicensePlate(id, dto);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(motorcycleDto);
    }

    [Fact]
    public async Task RemoveMotorcycle_ValidId_ShouldReturnNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();

        _motorcycleServiceMock
            .Setup(x => x.RemoveMotorcycleAsync(id))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RemoveMotorcycle(id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task RemoveMotorcycle_WithRentals_ShouldReturnBadRequest()
    {
        // Arrange
        var id = Guid.NewGuid();

        _motorcycleServiceMock
            .Setup(x => x.RemoveMotorcycleAsync(id))
            .ThrowsAsync(new InvalidOperationException("Não é possível remover uma moto que possui locações registradas."));

        // Act
        var result = await _controller.RemoveMotorcycle(id);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}

