using FluentValidation;
using RentABike.Application.DTOs;

namespace RentABike.Application.Validators;

public class CreateMotorcycleDTOValidator : AbstractValidator<CreateMotorcycleDTO>
{
    public CreateMotorcycleDTOValidator()
    {
        RuleFor(x => x.Year)
            .NotEmpty().WithMessage("O ano é obrigatório")
            .GreaterThan(1900).WithMessage("O ano deve ser maior que 1900")
            .LessThanOrEqualTo(DateTime.Now.Year + 1).WithMessage("O ano não pode ser no futuro");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("O modelo é obrigatório")
            .MaximumLength(100).WithMessage("O modelo deve ter no máximo 100 caracteres");

        RuleFor(x => x.LicensePlate)
            .NotEmpty().WithMessage("A placa é obrigatória")
            .MaximumLength(10).WithMessage("A placa deve ter no máximo 10 caracteres");
    }
}

