namespace RentABike.Domain.Entities;

public class DeliveryPerson
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string TaxIdNumber { get; private set; } = string.Empty;
    public DateTime BirthDate { get; private set; }
    public string LicenseNumber { get; private set; } = string.Empty;
    public LicenseType LicenseType { get; private set; }
    public string? LicenseImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected DeliveryPerson() { }

    public DeliveryPerson(string name, string taxIdNumber, DateTime birthDate, string licenseNumber, LicenseType licenseType)
    {
        Id = Guid.NewGuid();
        Name = name;
        TaxIdNumber = taxIdNumber;
        BirthDate = birthDate;
        LicenseNumber = licenseNumber;
        LicenseType = licenseType;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateLicenseImage(string url)
    {
        LicenseImageUrl = url;
    }

    public bool CanRent()
    {
        return LicenseType == LicenseType.A || LicenseType == LicenseType.AB;
    }
}

