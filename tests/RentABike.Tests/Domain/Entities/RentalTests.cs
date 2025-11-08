using RentABike.Domain.Entities;

namespace RentABike.Tests.Domain.Entities;

public class RentalTests
{
    [Fact]
    public void RegisterReturn_EarlyReturn_7DaysPlan_ShouldCalculateFine()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expectedEndDate = new DateTime(2024, 1, 8, 0, 0, 0, DateTimeKind.Utc); // 7 days plan
        var returnDate = new DateTime(2024, 1, 5, 0, 0, 0, DateTimeKind.Utc); // 3 days early

        var rental = new Rental(motorcycleId, deliveryPersonId, startDate, expectedEndDate, 7);

        // Act
        rental.RegisterReturn(returnDate);

        // Assert
        rental.ReturnDate.Should().Be(returnDate);
        rental.FineAmount.Should().NotBeNull();
        rental.FineAmount.Should().BeGreaterThan(0);
        rental.AdditionalAmount.Should().BeNull();

        // Expected: 3 unused days * R$30.00 = R$90.00, fine = 20% = R$18.00
        var expectedUnusedDays = 3;
        var expectedUnusedValue = expectedUnusedDays * 30.00m;
        var expectedFine = expectedUnusedValue * 0.20m;
        rental.FineAmount.Should().Be(expectedFine);
    }

    [Fact]
    public void RegisterReturn_EarlyReturn_15DaysPlan_ShouldCalculateFine()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expectedEndDate = new DateTime(2024, 1, 16, 0, 0, 0, DateTimeKind.Utc); // 15 days plan
        var returnDate = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc); // 6 days early

        var rental = new Rental(motorcycleId, deliveryPersonId, startDate, expectedEndDate, 15);

        // Act
        rental.RegisterReturn(returnDate);

        // Assert
        rental.ReturnDate.Should().Be(returnDate);
        rental.FineAmount.Should().NotBeNull();
        rental.FineAmount.Should().BeGreaterThan(0);
        rental.AdditionalAmount.Should().BeNull();

        // Expected: 6 unused days * R$28.00 = R$168.00, fine = 40% = R$67.20
        var expectedUnusedDays = 6;
        var expectedUnusedValue = expectedUnusedDays * 28.00m;
        var expectedFine = expectedUnusedValue * 0.40m;
        rental.FineAmount.Should().Be(expectedFine);
    }

    [Fact]
    public void RegisterReturn_LateReturn_ShouldCalculateAdditionalAmount()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expectedEndDate = new DateTime(2024, 1, 8, 0, 0, 0, DateTimeKind.Utc); // 7 days plan
        var returnDate = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc); // 2 days late

        var rental = new Rental(motorcycleId, deliveryPersonId, startDate, expectedEndDate, 7);

        // Act
        rental.RegisterReturn(returnDate);

        // Assert
        rental.ReturnDate.Should().Be(returnDate);
        rental.AdditionalAmount.Should().NotBeNull();
        rental.AdditionalAmount.Should().BeGreaterThan(0);
        rental.FineAmount.Should().BeNull();

        // Expected: 2 additional days * R$50.00 = R$100.00
        var expectedAdditionalDays = 2;
        var expectedAdditional = expectedAdditionalDays * 50.00m;
        rental.AdditionalAmount.Should().Be(expectedAdditional);
    }

    [Fact]
    public void RegisterReturn_OnTime_ShouldNotCalculateFineOrAdditional()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expectedEndDate = new DateTime(2024, 1, 8, 0, 0, 0, DateTimeKind.Utc); // 7 days plan (Jan 1 to Jan 8 = 8 days inclusive)
        var returnDate = expectedEndDate; // On time

        var rental = new Rental(motorcycleId, deliveryPersonId, startDate, expectedEndDate, 7);

        // Act
        rental.RegisterReturn(returnDate);

        // Assert
        rental.ReturnDate.Should().Be(returnDate);
        rental.FineAmount.Should().BeNull();
        rental.AdditionalAmount.Should().BeNull();

        // Expected: 8 days used (Jan 1 to Jan 8 inclusive) * R$30.00 = R$240.00
        var expectedDaysUsed = 8; // (Jan 8 - Jan 1).Days + 1 = 7 + 1 = 8
        var expectedTotal = expectedDaysUsed * 30.00m;
        rental.TotalAmount.Should().Be(expectedTotal);
    }

    [Fact]
    public void RegisterReturn_EarlyReturn_30DaysPlan_ShouldNotCalculateFine()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expectedEndDate = new DateTime(2024, 1, 31, 0, 0, 0, DateTimeKind.Utc); // 30 days plan
        var returnDate = new DateTime(2024, 1, 25, 0, 0, 0, DateTimeKind.Utc); // 6 days early

        var rental = new Rental(motorcycleId, deliveryPersonId, startDate, expectedEndDate, 30);

        // Act
        rental.RegisterReturn(returnDate);

        // Assert
        rental.ReturnDate.Should().Be(returnDate);
        rental.FineAmount.Should().Be(0); // Fine is 0 for plans other than 7 or 15 days
        rental.AdditionalAmount.Should().BeNull();
    }

    [Theory]
    [InlineData(7, 30.00)]
    [InlineData(15, 28.00)]
    [InlineData(30, 22.00)]
    [InlineData(45, 20.00)]
    [InlineData(50, 18.00)]
    public void CalculateDailyRate_ValidPlans_ShouldReturnCorrectRate(int planDays, decimal expectedRate)
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expectedEndDate = startDate.AddDays(planDays);

        // Act
        var rental = new Rental(motorcycleId, deliveryPersonId, startDate, expectedEndDate, planDays);

        // Assert
        rental.DailyRate.Should().Be(expectedRate);
    }
}

