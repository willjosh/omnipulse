using System;

using Application.Features.Inventory.Command.CreateInventory;
using Application.Features.Inventory.Command.UpdateInventory;
using Application.Features.Inventory.Query;

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

        CreateMap<Inventory, InventoryDetailDTO>()
            .ForMember(dest => dest.InventoryItemName, opt => opt.MapFrom(src => src.InventoryItem != null ? src.InventoryItem.ItemName : "Unknown Item"))
            .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.InventoryItemLocation != null ? src.InventoryItemLocation.LocationName : "Unknown Location"))
            .ForMember(dest => dest.LastRestockedDate, opt => opt.MapFrom(src => src.LastRestockedDate)); // Keep as DateTime?, don't convert to string

        CreateMap<UpdateInventoryCommand, Inventory>()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.InventoryID))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.LastRestockedDate, opt => opt.Ignore())
            .ForMember(dest => dest.InventoryTransactions, opt => opt.Ignore())
            .ForMember(dest => dest.InventoryItem, opt => opt.Ignore());
    }
}