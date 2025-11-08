using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentABike.Application.DTOs;
using RentABike.Application.Services;
using RentABike.Application.Services.Interfaces;
using RentABike.Domain.Entities;
using RentABike.Domain.Interfaces;

namespace RentABike.Tests.Application.Services;

public class DeliveryPersonServiceTests
{
    private readonly Mock<IDeliveryPersonRepository> _deliveryPersonRepositoryMock;
    private readonly Mock<IStorageService> _storageServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly DeliveryPersonService _service;

    public DeliveryPersonServiceTests()
    {
        _deliveryPersonRepositoryMock = new Mock<IDeliveryPersonRepository>();
        _storageServiceMock = new Mock<IStorageService>();
        _mapperMock = new Mock<IMapper>();

        _service = new DeliveryPersonService(
            _deliveryPersonRepositoryMock.Object,
            _storageServiceMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task CreateDeliveryPersonAsync_ValidData_ShouldCreate()
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

        var deliveryPerson = new DeliveryPerson(
            dto.Name,
            dto.TaxIdNumber,
            dto.BirthDate,
            dto.LicenseNumber,
            LicenseType.A
        );

        var deliveryPersonDto = new DeliveryPersonDTO
        {
            Id = deliveryPerson.Id,
            Name = deliveryPerson.Name,
            TaxIdNumber = deliveryPerson.TaxIdNumber,
            LicenseType = "A"
        };

        _deliveryPersonRepositoryMock
            .Setup(x => x.TaxIdNumberExistsAsync(dto.TaxIdNumber, null))
            .ReturnsAsync(false);

        _deliveryPersonRepositoryMock
            .Setup(x => x.LicenseNumberExistsAsync(dto.LicenseNumber, null))
            .ReturnsAsync(false);

        _deliveryPersonRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<DeliveryPerson>()))
            .ReturnsAsync(deliveryPerson);

        _mapperMock
            .Setup(x => x.Map<DeliveryPersonDTO>(It.IsAny<DeliveryPerson>()))
            .Returns(deliveryPersonDto);

        // Act
        var result = await _service.CreateDeliveryPersonAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        _deliveryPersonRepositoryMock.Verify(x => x.AddAsync(It.IsAny<DeliveryPerson>()), Times.Once);
    }

    [Fact]
    public async Task CreateDeliveryPersonAsync_DuplicateTaxIdNumber_ShouldThrowException()
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

        _deliveryPersonRepositoryMock
            .Setup(x => x.TaxIdNumberExistsAsync(dto.TaxIdNumber, null))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateDeliveryPersonAsync(dto));
        _deliveryPersonRepositoryMock.Verify(x => x.AddAsync(It.IsAny<DeliveryPerson>()), Times.Never);
    }

    [Fact]
    public async Task CreateDeliveryPersonAsync_DuplicateLicenseNumber_ShouldThrowException()
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

        _deliveryPersonRepositoryMock
            .Setup(x => x.TaxIdNumberExistsAsync(dto.TaxIdNumber, null))
            .ReturnsAsync(false);

        _deliveryPersonRepositoryMock
            .Setup(x => x.LicenseNumberExistsAsync(dto.LicenseNumber, null))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateDeliveryPersonAsync(dto));
        _deliveryPersonRepositoryMock.Verify(x => x.AddAsync(It.IsAny<DeliveryPerson>()), Times.Never);
    }

    [Fact]
    public async Task CreateDeliveryPersonAsync_WithImage_ShouldUploadImage()
    {
        // Arrange
        var dto = new CreateDeliveryPersonDTO
        {
            Name = "João Silva",
            TaxIdNumber = "12345678000190",
            BirthDate = new DateTime(1990, 1, 1),
            LicenseNumber = "12345678901",
            LicenseType = "A",
            LicenseImage = CreateMockFormFile("test.png", "image/png")
        };

        var deliveryPerson = new DeliveryPerson(
            dto.Name,
            dto.TaxIdNumber,
            dto.BirthDate,
            dto.LicenseNumber,
            LicenseType.A
        );

        var imageUrl = "https://example.com/image.png";

        _deliveryPersonRepositoryMock
            .Setup(x => x.TaxIdNumberExistsAsync(dto.TaxIdNumber, null))
            .ReturnsAsync(false);

        _deliveryPersonRepositoryMock
            .Setup(x => x.LicenseNumberExistsAsync(dto.LicenseNumber, null))
            .ReturnsAsync(false);

        _storageServiceMock
            .Setup(x => x.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(imageUrl);

        _deliveryPersonRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<DeliveryPerson>()))
            .ReturnsAsync(deliveryPerson);

        _mapperMock
            .Setup(x => x.Map<DeliveryPersonDTO>(It.IsAny<DeliveryPerson>()))
            .Returns(new DeliveryPersonDTO { Id = deliveryPerson.Id });

        // Act
        var result = await _service.CreateDeliveryPersonAsync(dto);

        // Assert
        result.Should().NotBeNull();
        _storageServiceMock.Verify(x => x.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetDeliveryPersonByIdAsync_ExistingId_ShouldReturnDeliveryPerson()
    {
        // Arrange
        var id = Guid.NewGuid();
        var deliveryPerson = new DeliveryPerson(
            "João Silva",
            "12345678000190",
            new DateTime(1990, 1, 1),
            "12345678901",
            LicenseType.A
        );

        var deliveryPersonDto = new DeliveryPersonDTO
        {
            Id = deliveryPerson.Id,
            Name = deliveryPerson.Name
        };

        _deliveryPersonRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(deliveryPerson);

        _mapperMock
            .Setup(x => x.Map<DeliveryPersonDTO>(deliveryPerson))
            .Returns(deliveryPersonDto);

        // Act
        var result = await _service.GetDeliveryPersonByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(deliveryPerson.Id);
    }

    [Fact]
    public async Task UpdateLicenseImageAsync_ValidFile_ShouldUpdate()
    {
        // Arrange
        var id = Guid.NewGuid();
        var deliveryPerson = new DeliveryPerson(
            "João Silva",
            "12345678000190",
            new DateTime(1990, 1, 1),
            "12345678901",
            LicenseType.A
        );

        var stream = new MemoryStream();
        var fileName = "test.png";
        var contentType = "image/png";
        var imageUrl = "https://example.com/image.png";

        _deliveryPersonRepositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(deliveryPerson);

        _storageServiceMock
            .Setup(x => x.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(imageUrl);

        _deliveryPersonRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<DeliveryPerson>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(x => x.Map<DeliveryPersonDTO>(It.IsAny<DeliveryPerson>()))
            .Returns(new DeliveryPersonDTO { Id = id, LicenseImageUrl = imageUrl });

        // Act
        var result = await _service.UpdateLicenseImageAsync(id, stream, fileName, contentType);

        // Assert
        result.Should().NotBeNull();
        result.LicenseImageUrl.Should().Be(imageUrl);
        _storageServiceMock.Verify(x => x.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _deliveryPersonRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<DeliveryPerson>()), Times.Once);
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

        return fileMock.Object;
    }
}

