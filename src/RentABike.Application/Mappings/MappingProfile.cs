using AutoMapper;
using RentABike.Application.DTOs;
using RentABike.Domain.Entities;

namespace RentABike.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Motorcycle mappings
        CreateMap<Motorcycle, MotorcycleDTO>();
        CreateMap<CreateMotorcycleDTO, Motorcycle>();

        // DeliveryPerson mappings
        CreateMap<DeliveryPerson, DeliveryPersonDTO>()
            .ForMember(dest => dest.LicenseType, opt => opt.MapFrom(src => src.LicenseType.ToString()));

        // Rental mappings
        CreateMap<Rental, RentalDTO>();
    }
}
