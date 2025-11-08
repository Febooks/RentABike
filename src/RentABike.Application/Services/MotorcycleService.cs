using AutoMapper;
using RentABike.Application.DTOs;
using RentABike.Application.Services.Interfaces;
using RentABike.Domain.Entities;
using RentABike.Domain.Events;
using RentABike.Domain.Interfaces;

namespace RentABike.Application.Services;

public class MotorcycleService : IMotorcycleService
{
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly IMessageBus _messageBus;
    private readonly IMapper _mapper;

    public MotorcycleService(
        IMotorcycleRepository motorcycleRepository,
        IRentalRepository rentalRepository,
        IMessageBus messageBus,
        IMapper mapper)
    {
        _motorcycleRepository = motorcycleRepository;
        _rentalRepository = rentalRepository;
        _messageBus = messageBus;
        _mapper = mapper;
    }

    public async Task<MotorcycleDTO> CreateMotorcycleAsync(CreateMotorcycleDTO dto)
    {
        if (await _motorcycleRepository.LicensePlateExistsAsync(dto.LicensePlate))
        {
            throw new InvalidOperationException("A placa já está cadastrada no sistema.");
        }

        var motorcycle = new Motorcycle(dto.Year, dto.Model, dto.LicensePlate);
        await _motorcycleRepository.AddAsync(motorcycle);

        var eventData = new MotorcycleRegisteredEvent
        {
            MotorcycleId = motorcycle.Id,
            Year = motorcycle.Year,
            Model = motorcycle.Model,
            LicensePlate = motorcycle.LicensePlate,
            RegistrationDate = motorcycle.CreatedAt
        };

        await _messageBus.PublishAsync(eventData);

        return _mapper.Map<MotorcycleDTO>(motorcycle);
    }

    public async Task<IEnumerable<MotorcycleDTO>> ListMotorcyclesAsync(string? licensePlate = null)
    {
        var motorcycles = await _motorcycleRepository.GetByLicensePlateFilterAsync(licensePlate);
        return _mapper.Map<IEnumerable<MotorcycleDTO>>(motorcycles);
    }

    public async Task<MotorcycleDTO?> GetMotorcycleByIdAsync(Guid id)
    {
        var motorcycle = await _motorcycleRepository.GetByIdAsync(id);
        return motorcycle == null ? null : _mapper.Map<MotorcycleDTO>(motorcycle);
    }

    public async Task<MotorcycleDTO> UpdateLicensePlateAsync(Guid id, UpdateMotorcycleLicensePlateDTO dto)
    {
        var motorcycle = await _motorcycleRepository.GetByIdAsync(id);
        if (motorcycle == null)
        {
            throw new InvalidOperationException("Moto não encontrada.");
        }

        if (await _motorcycleRepository.LicensePlateExistsAsync(dto.LicensePlate, id))
        {
            throw new InvalidOperationException("A placa já está cadastrada no sistema.");
        }

        motorcycle.UpdateLicensePlate(dto.LicensePlate);
        await _motorcycleRepository.UpdateAsync(motorcycle);

        return _mapper.Map<MotorcycleDTO>(motorcycle);
    }

    public async Task RemoveMotorcycleAsync(Guid id)
    {
        var motorcycle = await _motorcycleRepository.GetByIdAsync(id);
        if (motorcycle == null)
        {
            throw new InvalidOperationException("Moto não encontrada.");
        }

        if (await _rentalRepository.HasRentalsByMotorcycleIdAsync(id))
        {
            throw new InvalidOperationException("Não é possível remover uma moto que possui locações registradas.");
        }

        await _motorcycleRepository.DeleteAsync(motorcycle);
    }
}

