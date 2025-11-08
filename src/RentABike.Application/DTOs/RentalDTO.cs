namespace RentABike.Application.DTOs;

public class RentalDTO
{
    public Guid Id { get; set; }
    public Guid MotorcycleId { get; set; }
    public Guid DeliveryPersonId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime ExpectedEndDate { get; set; }
    public int PlanDays { get; set; }
    public decimal DailyRate { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime? ReturnDate { get; set; }
    public decimal? FineAmount { get; set; }
    public decimal? AdditionalAmount { get; set; }
}

public class CreateRentalDTO
{
    public Guid MotorcycleId { get; set; }
    public Guid DeliveryPersonId { get; set; }
    public int PlanDays { get; set; }
}

public class ReturnRentalDTO
{
    public DateTime ReturnDate { get; set; }
}

public class CalculateRentalDTO
{
    public DateTime ReturnDate { get; set; }
}

