using RentABike.Domain.Entities;

namespace RentABike.Tests.Domain.Entities;

public class MotorcycleTests
{
    [Fact]
    public void UpdateLicensePlate_ValidPlate_ShouldUpdate()
    {
        // Arrange
        var motorcycle = new Motorcycle(2024, "Honda CB 600F", "ABC1234");

        // Act
        motorcycle.UpdateLicensePlate("XYZ5678");

        // Assert
        motorcycle.LicensePlate.Should().Be("XYZ5678");
    }

    [Fact]
    public void UpdateLicensePlate_EmptyPlate_ShouldThrowException()
    {
        // Arrange
        var motorcycle = new Motorcycle(2024, "Honda CB 600F", "ABC1234");

        // Act & Assert
        var action = () => motorcycle.UpdateLicensePlate("");
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateLicensePlate_WhiteSpacePlate_ShouldThrowException()
    {
        // Arrange
        var motorcycle = new Motorcycle(2024, "Honda CB 600F", "ABC1234");

        // Act & Assert
        var action = () => motorcycle.UpdateLicensePlate("   ");
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ValidData_ShouldCreateMotorcycle()
    {
        // Arrange & Act
        var motorcycle = new Motorcycle(2024, "Honda CB 600F", "ABC1234");

        // Assert
        motorcycle.Year.Should().Be(2024);
        motorcycle.Model.Should().Be("Honda CB 600F");
        motorcycle.LicensePlate.Should().Be("ABC1234");
        motorcycle.Id.Should().NotBeEmpty();
        motorcycle.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}

