using Microsoft.AspNetCore.Mvc;
using RentABike.API.Controllers;
using RentABike.Application.DTOs;
using RentABike.Application.Services.Interfaces;

namespace RentABike.Tests.API.Controllers;

public class RentalsControllerTests
{
    private readonly Mock<IRentalService> _rentalServiceMock;
    private readonly RentalsController _controller;

    public RentalsControllerTests()
    {
        _rentalServiceMock = new Mock<IRentalService>();
        _controller = new RentalsController(_rentalServiceMock.Object);
    }

    [Fact]
    public async Task CreateRental_ValidData_ShouldReturnCreated()
    {
        // Arrange
        var dto = new CreateRentalDTO
        {
            MotorcycleId = Guid.NewGuid(),
            DeliveryPersonId = Guid.NewGuid(),
            PlanDays = 7
        };

        var rentalDto = new RentalDTO
        {
            Id = Guid.NewGuid(),
            MotorcycleId = dto.MotorcycleId,
            DeliveryPersonId = dto.DeliveryPersonId,
            PlanDays = dto.PlanDays
        };

        _rentalServiceMock
            .Setup(x => x.CreateRentalAsync(dto))
            .ReturnsAsync(rentalDto);

        // Act
        var result = await _controller.CreateRental(dto);

        // Assert
        result.Should().NotBeNull();
        var createdAtResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtResult.Value.Should().BeEquivalentTo(rentalDto);
    }

    [Fact]
    public async Task CreateRental_InvalidOperationException_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new CreateRentalDTO
        {
            MotorcycleId = Guid.NewGuid(),
            DeliveryPersonId = Guid.NewGuid(),
            PlanDays = 7
        };

        _rentalServiceMock
            .Setup(x => x.CreateRentalAsync(dto))
            .ThrowsAsync(new InvalidOperationException("Moto não encontrada."));

        // Act
        var result = await _controller.CreateRental(dto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetRental_ExistingId_ShouldReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var rentalDto = new RentalDTO
        {
            Id = id,
            MotorcycleId = Guid.NewGuid(),
            DeliveryPersonId = Guid.NewGuid(),
            PlanDays = 7
        };

        _rentalServiceMock
            .Setup(x => x.GetRentalByIdAsync(id))
            .ReturnsAsync(rentalDto);

        // Act
        var result = await _controller.GetRental(id);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(rentalDto);
    }

    [Fact]
    public async Task GetRental_NonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        _rentalServiceMock
            .Setup(x => x.GetRentalByIdAsync(id))
            .ReturnsAsync((RentalDTO?)null);

        // Act
        var result = await _controller.GetRental(id);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ReturnRental_ValidData_ShouldReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new ReturnRentalDTO { ReturnDate = DateTime.UtcNow };
        var rentalDto = new RentalDTO
        {
            Id = id,
            ReturnDate = dto.ReturnDate
        };

        _rentalServiceMock
            .Setup(x => x.ReturnRentalAsync(id, dto))
            .ReturnsAsync(rentalDto);

        // Act
        var result = await _controller.ReturnRental(id, dto);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(rentalDto);
    }

    [Fact]
    public async Task ReturnRental_ArgumentException_ShouldReturnBadRequest()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new ReturnRentalDTO { ReturnDate = DateTime.UtcNow };

        _rentalServiceMock
            .Setup(x => x.ReturnRentalAsync(id, dto))
            .ThrowsAsync(new ArgumentException("Locação não encontrada."));

        // Act
        var result = await _controller.ReturnRental(id, dto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CalculateRental_ValidData_ShouldReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new CalculateRentalDTO { ReturnDate = DateTime.UtcNow };
        var rentalDto = new RentalDTO
        {
            Id = id,
            TotalAmount = 210.00m
        };

        _rentalServiceMock
            .Setup(x => x.CalculateRentalAsync(id, dto))
            .ReturnsAsync(rentalDto);

        // Act
        var result = await _controller.CalculateRental(id, dto);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(rentalDto);
    }

    [Fact]
    public async Task CalculateRental_ArgumentException_ShouldReturnBadRequest()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new CalculateRentalDTO { ReturnDate = DateTime.UtcNow };

        _rentalServiceMock
            .Setup(x => x.CalculateRentalAsync(id, dto))
            .ThrowsAsync(new ArgumentException("Locação não encontrada."));

        // Act
        var result = await _controller.CalculateRental(id, dto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}

