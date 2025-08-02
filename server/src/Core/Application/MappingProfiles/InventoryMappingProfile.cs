using System;

using Application.Features.Inventory.Command.CreateInventory;

using AutoMapper;

using Domain.Entities;

namespace Application.MappingProfiles;

public class InventoryMappingProfile : Profile
{
    public InventoryMappingProfile()
    {
        CreateMap<CreateInventoryCommand, Inventory>()
           .ForMember(dest => dest.ID, opt => opt.Ignore())
           .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
           .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
           .ForMember(dest => dest.LastRestockedDate, opt => opt.Ignore())
           .ForMember(dest => dest.InventoryTransactions, opt => opt.Ignore())
           .ForMember(dest => dest.InventoryItem, opt => opt.Ignore())
           .ForMember(dest => dest.InventoryItemLocation, opt => opt.Ignore());
    }
}