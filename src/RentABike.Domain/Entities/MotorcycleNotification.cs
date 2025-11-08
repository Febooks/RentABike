namespace RentABike.Domain.Entities;

public class MotorcycleNotification
{
    public Guid Id { get; private set; }
    public Guid MotorcycleId { get; private set; }
    public int Year { get; private set; }
    public string Model { get; private set; } = string.Empty;
    public string LicensePlate { get; private set; } = string.Empty;
    public DateTime NotificationDate { get; private set; }

    protected MotorcycleNotification() { }

    public MotorcycleNotification(Guid motorcycleId, int year, string model, string licensePlate)
    {
        Id = Guid.NewGuid();
        MotorcycleId = motorcycleId;
        Year = year;
        Model = model;
        LicensePlate = licensePlate;
        NotificationDate = DateTime.UtcNow;
    }
}

