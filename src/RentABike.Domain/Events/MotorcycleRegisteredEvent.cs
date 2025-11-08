namespace RentABike.Domain.Events;

public class MotorcycleRegisteredEvent
{
    public Guid MotorcycleId { get; set; }
    public int Year { get; set; }
    public string Model { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
}

