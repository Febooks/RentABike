namespace RentABike.Application.DTOs;

public class MotorcycleDTO
{
    public Guid Id { get; set; }
    public int Year { get; set; }
    public string Model { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
}

public class CreateMotorcycleDTO
{
    public int Year { get; set; }
    public string Model { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
}

public class UpdateMotorcycleLicensePlateDTO
{
    public string LicensePlate { get; set; } = string.Empty;
}

