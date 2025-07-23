using Application.Features.InventoryItemLocations.Command;
using Application.Features.InventoryItemLocations.Query.GetAllInventoryItemLocation;

using AutoMapper;

using Domain.Entities;

namespace Application.MappingProfiles;

public class InventoryItemLocationMappingProfile : Profile
{
    public InventoryItemLocationMappingProfile()
    {
        CreateMap<CreateInventoryItemLocationCommand, InventoryItemLocation>()
            .ForMember(dest => dest.ID, opt => opt.Ignore()) // Don't map ID - will be auto-generated
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Handled by BaseEntity
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // Handled by BaseEntity
            .ForMember(dest => dest.Inventories, opt => opt.Ignore()); // Navigation collection

        CreateMap<InventoryItemLocation, GetAllInventoryItemLocationDTO>();
    }
}