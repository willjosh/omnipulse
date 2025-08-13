using Application.Features.FuelPurchases.Command.CreateFuelPurchase;
using Application.Features.FuelPurchases.Command.UpdateFuelPurchase;
using Application.Features.FuelPurchases.Query;

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

        CreateMap<FuelPurchase, FuelPurchaseDTO>(MemberList.Destination);
    }
}