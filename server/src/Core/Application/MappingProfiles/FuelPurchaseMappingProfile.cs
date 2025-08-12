using Application.Features.FuelLogging.Command.CreateFuelPurchase;
using Application.Features.FuelLogging.Command.UpdateFuelPurchase;
using Application.Features.FuelPurchases.Query.GetAllFuelPurchases;

using AutoMapper;

using Domain.Entities;

namespace Application.MappingProfiles;

public class FuelPurchaseMappingProfile : Profile
{
    public FuelPurchaseMappingProfile()
    {
        CreateMap<CreateFuelPurchaseCommand, FuelPurchase>(MemberList.Destination)
            .ForMember(dest => dest.ID, opt => opt.Ignore()) // Don't map ID - will be auto-generated
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Handled by BaseEntity
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // Handled by BaseEntity
            .ForMember(dest => dest.Vehicle, opt => opt.Ignore()) // Navigation property
            .ForMember(dest => dest.User, opt => opt.Ignore()); // Navigation property

        CreateMap<UpdateFuelPurchaseCommand, FuelPurchase>(MemberList.Destination)
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ForMember(dest => dest.TotalCost, opt => opt.Ignore()) // Will be recalculated
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Vehicle, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<FuelPurchase, GetAllFuelPurchasesDTO>(MemberList.Destination)
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.VehicleId, opt => opt.MapFrom(src => src.VehicleId))
            .ForMember(dest => dest.FuelStation, opt => opt.MapFrom(src => src.FuelStation))
            .ForMember(dest => dest.PurchasedByUserId, opt => opt.MapFrom(src => src.PurchasedByUserId))
            .ForMember(dest => dest.PurchaseDate, opt => opt.MapFrom(src => src.PurchaseDate))
            .ForMember(dest => dest.OdometerReading, opt => opt.MapFrom(src => src.OdometerReading))
            .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.Volume))
            .ForMember(dest => dest.PricePerUnit, opt => opt.MapFrom(src => src.PricePerUnit))
            .ForMember(dest => dest.TotalCost, opt => opt.MapFrom(src => src.TotalCost))
            ;
    }
}