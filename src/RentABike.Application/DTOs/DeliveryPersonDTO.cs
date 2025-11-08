using Microsoft.AspNetCore.Http;

namespace RentABike.Application.DTOs;

public class DeliveryPersonDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TaxIdNumber { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public string LicenseType { get; set; } = string.Empty;
    public string? LicenseImageUrl { get; set; }
}

public class CreateDeliveryPersonDTO
{
    public string Name { get; set; } = string.Empty;
    public string TaxIdNumber { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public string LicenseType { get; set; } = string.Empty;
    public IFormFile? LicenseImage { get; set; }
}

