using AutoMapper;
using RentABike.Application.DTOs;
using RentABike.Application.Services.Interfaces;
using RentABike.Domain.Entities;
using RentABike.Domain.Interfaces;
using System.IO;

namespace RentABike.Application.Services;

public class DeliveryPersonService : IDeliveryPersonService
{
    private readonly IDeliveryPersonRepository _deliveryPersonRepository;
    private readonly IStorageService _storageService;
    private readonly IMapper _mapper;

    public DeliveryPersonService(
        IDeliveryPersonRepository deliveryPersonRepository,
        IStorageService storageService,
        IMapper mapper)
    {
        _deliveryPersonRepository = deliveryPersonRepository;
        _storageService = storageService;
        _mapper = mapper;
    }

    public async Task<DeliveryPersonDTO> CreateDeliveryPersonAsync(CreateDeliveryPersonDTO dto)
    {
        if (await _deliveryPersonRepository.TaxIdNumberExistsAsync(dto.TaxIdNumber))
        {
            throw new InvalidOperationException("O CNPJ já está cadastrado no sistema.");
        }

        if (await _deliveryPersonRepository.LicenseNumberExistsAsync(dto.LicenseNumber))
        {
            throw new InvalidOperationException("O número da CNH já está cadastrado no sistema.");
        }

        if (!Enum.TryParse<LicenseType>(dto.LicenseType, true, out var licenseType) ||
            (licenseType != LicenseType.A && licenseType != LicenseType.B && licenseType != LicenseType.AB))
        {
            throw new InvalidOperationException("Tipo de CNH inválido. Os tipos válidos são: A, B ou AB.");
        }

        var birthDate = dto.BirthDate;
        if (birthDate.Kind != DateTimeKind.Utc)
        {
            birthDate = birthDate.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(birthDate, DateTimeKind.Utc)
                : birthDate.ToUniversalTime();
        }

        var deliveryPerson = new DeliveryPerson(
            dto.Name,
            dto.TaxIdNumber,
            birthDate,
            dto.LicenseNumber,
            licenseType
        );

        if (dto.LicenseImage != null && !string.IsNullOrEmpty(dto.LicenseImage.FileName))
        {
            var extension = Path.GetExtension(dto.LicenseImage.FileName).ToLowerInvariant();
            if (extension != ".png" && extension != ".bmp")
            {
                throw new InvalidOperationException("O formato do arquivo deve ser PNG ou BMP");
            }

            using var imageStream = new MemoryStream();
            await dto.LicenseImage.CopyToAsync(imageStream);
            imageStream.Position = 0;

            var url = await _storageService.UploadFileAsync(imageStream, Guid.NewGuid().ToString(), dto.LicenseImage.ContentType ?? "image/png");
            deliveryPerson.UpdateLicenseImage(url);
        }

        await _deliveryPersonRepository.AddAsync(deliveryPerson);

        return _mapper.Map<DeliveryPersonDTO>(deliveryPerson);
    }

    public async Task<DeliveryPersonDTO?> GetDeliveryPersonByIdAsync(Guid id)
    {
        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(id);
        return deliveryPerson == null ? null : _mapper.Map<DeliveryPersonDTO>(deliveryPerson);
    }

    public async Task<DeliveryPersonDTO> UpdateLicenseImageAsync(Guid id, Stream file, string fileName, string contentType)
    {
        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(id);
        if (deliveryPerson == null)
        {
            throw new ArgumentException("Entregador não encontrado.");
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (extension != ".png" && extension != ".bmp")
        {
            throw new ArgumentException("O formato do arquivo deve ser PNG ou BMP.");
        }

        if (!string.IsNullOrEmpty(deliveryPerson.LicenseImageUrl))
        {
            await _storageService.DeleteFileAsync(deliveryPerson.LicenseImageUrl);
        }

        var url = await _storageService.UploadFileAsync(file, Guid.NewGuid().ToString(), contentType);

        deliveryPerson.UpdateLicenseImage(url);
        await _deliveryPersonRepository.UpdateAsync(deliveryPerson);

        return _mapper.Map<DeliveryPersonDTO>(deliveryPerson);
    }
}

