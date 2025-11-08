using FluentValidation;
using RentABike.Application.DTOs;

namespace RentABike.Application.Validators;

public class CreateRentalDTOValidator : AbstractValidator<CreateRentalDTO>
{
    public CreateRentalDTOValidator()
    {
        RuleFor(x => x.MotorcycleId)
            .NotEmpty().WithMessage("O ID da moto é obrigatório");

        RuleFor(x => x.DeliveryPersonId)
            .NotEmpty().WithMessage("O ID do entregador é obrigatório");

        RuleFor(x => x.PlanDays)
            .NotEmpty().WithMessage("O plano em dias é obrigatório")
            .Must(p => new[] { 7, 15, 30, 45, 50 }.Contains(p))
            .WithMessage("O plano deve ser 7, 15, 30, 45 ou 50 dias");
    }
}

