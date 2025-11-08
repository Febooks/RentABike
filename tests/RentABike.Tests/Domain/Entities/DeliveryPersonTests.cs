using RentABike.Domain.Entities;

namespace RentABike.Tests.Domain.Entities;

public class DeliveryPersonTests
{
    [Fact]
    public void CanRent_LicenseTypeA_ShouldReturnTrue()
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
        var result = deliveryPerson.CanRent();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanRent_LicenseTypeAB_ShouldReturnTrue()
    {
        // Arrange
        var deliveryPerson = new DeliveryPerson(
            "João Silva",
            "12345678000190",
            new DateTime(1990, 1, 1),
            "12345678901",
            LicenseType.AB
        );

        // Act
        var result = deliveryPerson.CanRent();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanRent_LicenseTypeB_ShouldReturnFalse()
    {
        // Arrange
        var deliveryPerson = new DeliveryPerson(
            "João Silva",
            "12345678000190",
            new DateTime(1990, 1, 1),
            "12345678901",
            LicenseType.B
        );

        // Act
        var result = deliveryPerson.CanRent();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void UpdateLicenseImage_ValidUrl_ShouldUpdate()
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
        deliveryPerson.UpdateLicenseImage("https://example.com/image.png");

        // Assert
        deliveryPerson.LicenseImageUrl.Should().Be("https://example.com/image.png");
    }

    [Fact]
    public void Constructor_ValidData_ShouldCreateDeliveryPerson()
    {
        // Arrange & Act
        var deliveryPerson = new DeliveryPerson(
            "João Silva",
            "12345678000190",
            new DateTime(1990, 1, 1),
            "12345678901",
            LicenseType.A
        );

        // Assert
        deliveryPerson.Name.Should().Be("João Silva");
        deliveryPerson.TaxIdNumber.Should().Be("12345678000190");
        deliveryPerson.BirthDate.Should().Be(new DateTime(1990, 1, 1));
        deliveryPerson.LicenseNumber.Should().Be("12345678901");
        deliveryPerson.LicenseType.Should().Be(LicenseType.A);
        deliveryPerson.Id.Should().NotBeEmpty();
        deliveryPerson.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}

