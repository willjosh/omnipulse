using System;
using Application.Features.FuelLogging.Command.CreateFuelPurchase;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles;

public class FuelPurchaseMappingProfile : Profile
{
    public FuelPurchaseMappingProfile()
    {
        CreateMap<CreateFuelPurchaseCommand, FuelPurchase>()
            .ForMember(dest => dest.ID, opt => opt.Ignore()) // Don't map ID - will be auto-generated
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Handled by BaseEntity
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()); // Handled by BaseEntity
    }
}