using AutoMapper;
using RentABike.Application.DTOs;
using RentABike.Application.Services.Interfaces;
using RentABike.Domain.Entities;
using RentABike.Domain.Interfaces;

namespace RentABike.Application.Services;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly IDeliveryPersonRepository _deliveryPersonRepository;
    private readonly IMapper _mapper;

    public RentalService(
        IRentalRepository rentalRepository,
        IMotorcycleRepository motorcycleRepository,
        IDeliveryPersonRepository deliveryPersonRepository,
        IMapper mapper)
    {
        _rentalRepository = rentalRepository;
        _motorcycleRepository = motorcycleRepository;
        _deliveryPersonRepository = deliveryPersonRepository;
        _mapper = mapper;
    }

    public async Task<RentalDTO> CreateRentalAsync(CreateRentalDTO dto)
    {
        var motorcycle = await _motorcycleRepository.GetByIdAsync(dto.MotorcycleId);
        if (motorcycle == null)
        {
            throw new InvalidOperationException("Moto não encontrada.");
        }

        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(dto.DeliveryPersonId);
        if (deliveryPerson == null)
        {
            throw new InvalidOperationException("Entregador não encontrado.");
        }

        if (!deliveryPerson.CanRent())
        {
            throw new InvalidOperationException("Somente entregadores habilitados na categoria A podem efetuar uma locação.");
        }

        var activeRental = await _rentalRepository.GetActiveByDeliveryPersonIdAsync(dto.DeliveryPersonId);
        if (activeRental != null)
        {
            throw new InvalidOperationException("O entregador já possui uma locação ativa.");
        }

        var validPlans = new[] { 7, 15, 30, 45, 50 };
        if (!validPlans.Contains(dto.PlanDays))
        {
            throw new InvalidOperationException($"Plano de {dto.PlanDays} dias não é válido. Planos disponíveis: 7, 15, 30, 45, 50 dias.");
        }

        var startDate = DateTime.UtcNow.Date.AddDays(1);
        var expectedEndDate = startDate.AddDays(dto.PlanDays);

        var rental = new Rental(
            dto.MotorcycleId,
            dto.DeliveryPersonId,
            startDate,
            expectedEndDate,
            dto.PlanDays
        );

        await _rentalRepository.AddAsync(rental);

        return _mapper.Map<RentalDTO>(rental);
    }

    public async Task<RentalDTO?> GetRentalByIdAsync(Guid id)
    {
        var rental = await _rentalRepository.GetByIdAsync(id);
        return rental == null ? null : _mapper.Map<RentalDTO>(rental);
    }

    public async Task<RentalDTO> ReturnRentalAsync(Guid id, ReturnRentalDTO dto)
    {
        var rental = await _rentalRepository.GetByIdAsync(id);
        if (rental == null)
        {
            throw new ArgumentException("Locação não encontrada.");
        }

        var returnDate = dto.ReturnDate;
        if (returnDate.Kind != DateTimeKind.Utc)
        {
            returnDate = returnDate.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(returnDate, DateTimeKind.Utc)
                : returnDate.ToUniversalTime();
        }

        rental.RegisterReturn(returnDate);
        await _rentalRepository.UpdateAsync(rental);

        return _mapper.Map<RentalDTO>(rental);
    }

    public async Task<RentalDTO> CalculateRentalAsync(Guid id, CalculateRentalDTO dto)
    {
        var rental = await _rentalRepository.GetByIdAsync(id);
        if (rental == null)
        {
            throw new ArgumentException("Locação não encontrada.");
        }

        var returnDate = dto.ReturnDate;
        if (returnDate.Kind != DateTimeKind.Utc)
        {
            returnDate = returnDate.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(returnDate, DateTimeKind.Utc)
                : returnDate.ToUniversalTime();
        }

        var rentalCalculation = new Rental(
            rental.MotorcycleId,
            rental.DeliveryPersonId,
            rental.StartDate,
            rental.ExpectedEndDate,
            rental.PlanDays
        );

        rentalCalculation.RegisterReturn(returnDate);

        var calculatedDto = _mapper.Map<RentalDTO>(rentalCalculation);
        calculatedDto.Id = rental.Id;

        return calculatedDto;
    }
}

