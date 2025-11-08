using FluentValidation;
using RentABike.Application.DTOs;

namespace RentABike.Application.Validators;

public class CreateDeliveryPersonDTOValidator : AbstractValidator<CreateDeliveryPersonDTO>
{
    public CreateDeliveryPersonDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório")
            .MaximumLength(200).WithMessage("O nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.TaxIdNumber)
            .NotEmpty().WithMessage("O CNPJ é obrigatório")
            .MaximumLength(18).WithMessage("O CNPJ deve ter no máximo 18 caracteres");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("A data de nascimento é obrigatória")
            .LessThan(DateTime.Now).WithMessage("A data de nascimento deve ser no passado");

        RuleFor(x => x.LicenseNumber)
            .NotEmpty().WithMessage("O número da CNH é obrigatório")
            .MaximumLength(20).WithMessage("O número da CNH deve ter no máximo 20 caracteres");

        RuleFor(x => x.LicenseType)
            .NotEmpty().WithMessage("O tipo da CNH é obrigatório")
            .Must(t => t == "A" || t == "B" || t == "AB")
            .WithMessage("O tipo da CNH deve ser A, B ou AB");
    }
}

