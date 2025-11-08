namespace RentABike.Domain.Entities;

public class Rental
{
    public Guid Id { get; private set; }
    public Guid MotorcycleId { get; private set; }
    public Guid DeliveryPersonId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public DateTime ExpectedEndDate { get; private set; }
    public int PlanDays { get; private set; }
    public decimal DailyRate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReturnDate { get; private set; }
    public decimal? FineAmount { get; private set; }
    public decimal? AdditionalAmount { get; private set; }

    protected Rental() { }

    public Rental(Guid motorcycleId, Guid deliveryPersonId, DateTime startDate, DateTime expectedEndDate, int planDays)
    {
        Id = Guid.NewGuid();
        MotorcycleId = motorcycleId;
        DeliveryPersonId = deliveryPersonId;
        StartDate = startDate;
        ExpectedEndDate = expectedEndDate;
        PlanDays = planDays;
        CreatedAt = DateTime.UtcNow;
        EndDate = expectedEndDate;

        DailyRate = CalculateDailyRate(planDays);
        TotalAmount = CalculateTotalAmount(planDays, DailyRate);
    }

    public void RegisterReturn(DateTime returnDate)
    {
        ReturnDate = returnDate;
        EndDate = returnDate;

        var returnDateOnly = returnDate.Date;
        var expectedEndDateOnly = ExpectedEndDate.Date;
        var startDateOnly = StartDate.Date;

        if (returnDateOnly < expectedEndDateOnly)
        {
            AdditionalAmount = null;

            var daysUsed = (returnDateOnly - startDateOnly).Days + 1;
            var daysNotUsed = (expectedEndDateOnly - returnDateOnly).Days;
            var unusedDaysValue = daysNotUsed * DailyRate;
            
            FineAmount = CalculateEarlyReturnFine(PlanDays, unusedDaysValue);
            TotalAmount = daysUsed * DailyRate + (FineAmount ?? 0);
        }
        else if (returnDateOnly > expectedEndDateOnly)
        {
            FineAmount = null;

            var additionalDays = (returnDateOnly - expectedEndDateOnly).Days;

            AdditionalAmount = additionalDays * 50.00m;
            TotalAmount = PlanDays * DailyRate + (AdditionalAmount ?? 0);
        }
        else
        {
            FineAmount = null;
            AdditionalAmount = null;

            var daysUsed = (returnDateOnly - startDateOnly).Days + 1;
            
            TotalAmount = daysUsed * DailyRate;
        }
    }

    private static decimal CalculateDailyRate(int planDays)
    {
        return planDays switch
        {
            7 => 30.00m,
            15 => 28.00m,
            30 => 22.00m,
            45 => 20.00m,
            50 => 18.00m,
            _ => throw new ArgumentException($"Plano de {planDays} dias não é válido", nameof(planDays))
        };
    }

    private static decimal CalculateTotalAmount(int planDays, decimal dailyRate)
    {
        return planDays * dailyRate;
    }

    private static decimal CalculateEarlyReturnFine(int planDays, decimal unusedDaysValue)
    {
        return planDays switch
        {
            7 => unusedDaysValue * 0.20m,
            15 => unusedDaysValue * 0.40m,
            _ => 0m
        };
    }
}

