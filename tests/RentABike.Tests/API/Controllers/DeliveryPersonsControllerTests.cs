using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentABike.API.Controllers;
using RentABike.Application.DTOs;
using RentABike.Application.Services.Interfaces;

namespace RentABike.Tests.API.Controllers;

public class DeliveryPersonsControllerTests
{
    private readonly Mock<IDeliveryPersonService> _deliveryPersonServiceMock;
    private readonly DeliveryPersonsController _controller;

    public DeliveryPersonsControllerTests()
    {
        _deliveryPersonServiceMock = new Mock<IDeliveryPersonService>();
        _controller = new DeliveryPersonsController(_deliveryPersonServiceMock.Object);
    }

    [Fact]
    public async Task CreateDeliveryPerson_ValidData_ShouldReturnCreated()
    {
        // Arrange
        var dto = new CreateDeliveryPersonDTO
        {
            Name = "João Silva",
            TaxIdNumber = "12345678000190",
            BirthDate = new DateTime(1990, 1, 1),
            LicenseNumber = "12345678901",
            LicenseType = "A"
        };

        var deliveryPersonDto = new DeliveryPersonDTO
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            TaxIdNumber = dto.TaxIdNumber,
            LicenseType = dto.LicenseType
        };

        _deliveryPersonServiceMock
            .Setup(x => x.CreateDeliveryPersonAsync(dto))
            .ReturnsAsync(deliveryPersonDto);

        // Act
        var result = await _controller.CreateDeliveryPerson(dto);

        // Assert
        result.Should().NotBeNull();
        var createdAtResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtResult.Value.Should().BeEquivalentTo(deliveryPersonDto);
    }

    [Fact]
    public async Task CreateDeliveryPerson_InvalidOperationException_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new CreateDeliveryPersonDTO
        {
            Name = "João Silva",
            TaxIdNumber = "12345678000190",
            BirthDate = new DateTime(1990, 1, 1),
            LicenseNumber = "12345678901",
            LicenseType = "A"
        };

        _deliveryPersonServiceMock
            .Setup(x => x.CreateDeliveryPersonAsync(dto))
            .ThrowsAsync(new InvalidOperationException("O CNPJ já está cadastrado no sistema."));

        // Act
        var result = await _controller.CreateDeliveryPerson(dto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetDeliveryPerson_ExistingId_ShouldReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var deliveryPersonDto = new DeliveryPersonDTO
        {
            Id = id,
            Name = "João Silva",
            TaxIdNumber = "12345678000190"
        };

        _deliveryPersonServiceMock
            .Setup(x => x.GetDeliveryPersonByIdAsync(id))
            .ReturnsAsync(deliveryPersonDto);

        // Act
        var result = await _controller.GetDeliveryPerson(id);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(deliveryPersonDto);
    }

    [Fact]
    public async Task GetDeliveryPerson_NonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        _deliveryPersonServiceMock
            .Setup(x => x.GetDeliveryPersonByIdAsync(id))
            .ReturnsAsync((DeliveryPersonDTO?)null);

        // Act
        var result = await _controller.GetDeliveryPerson(id);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task UpdateLicenseImage_ValidFile_ShouldReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var file = CreateMockFormFile("test.png", "image/png");
        var deliveryPersonDto = new DeliveryPersonDTO
        {
            Id = id,
            LicenseImageUrl = "https://example.com/image.png"
        };

        _deliveryPersonServiceMock
            .Setup(x => x.UpdateLicenseImageAsync(id, It.IsAny<Stream>(), file.FileName, file.ContentType))
            .ReturnsAsync(deliveryPersonDto);

        // Act
        var result = await _controller.UpdateLicenseImage(id, file);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(deliveryPersonDto);
    }

    [Fact]
    public async Task UpdateLicenseImage_NullFile_ShouldReturnBadRequest()
    {
        // Arrange
        var id = Guid.NewGuid();
        IFormFile? file = null;

        // Act
        var result = await _controller.UpdateLicenseImage(id, file!);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateLicenseImage_EmptyFile_ShouldReturnBadRequest()
    {
        // Arrange
        var id = Guid.NewGuid();
        var file = CreateMockFormFile("test.png", "image/png");
        file = new Mock<IFormFile>().Object;
        Mock.Get(file).Setup(f => f.Length).Returns(0);

        // Act
        var result = await _controller.UpdateLicenseImage(id, file);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    private IFormFile CreateMockFormFile(string fileName, string contentType)
    {
        var fileMock = new Mock<IFormFile>();
        var content = "Hello World from a Fake File";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;

        fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
        fileMock.Setup(_ => _.FileName).Returns(fileName);
        fileMock.Setup(_ => _.Length).Returns(ms.Length);
        fileMock.Setup(_ => _.ContentType).Returns(contentType);
        fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns((Stream target, CancellationToken ct) => ms.CopyToAsync(target, ct));

        return fileMock.Object;
    }
}

