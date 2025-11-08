namespace RentABike.Domain.Entities;

public class Motorcycle
{
    public Guid Id { get; private set; }
    public int Year { get; private set; }
    public string Model { get; private set; } = string.Empty;
    public string LicensePlate { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    protected Motorcycle() { }

    public Motorcycle(int year, string model, string licensePlate)
    {
        Id = Guid.NewGuid();
        Year = year;
        Model = model;
        LicensePlate = licensePlate;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateLicensePlate(string newLicensePlate)
    {
        if (string.IsNullOrWhiteSpace(newLicensePlate))
            throw new ArgumentException("A placa não pode ser vazia", nameof(newLicensePlate));

        LicensePlate = newLicensePlate;
    }
}

