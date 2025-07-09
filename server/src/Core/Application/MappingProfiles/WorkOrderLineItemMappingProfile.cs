using System;

using Application.Features.WorkOrderLineItem.Models;

using AutoMapper;

using Domain.Entities;

namespace Application.MappingProfiles;

public class WorkOrderLineItemMappingProfile : Profile
{
    public WorkOrderLineItemMappingProfile()
    {
        CreateMap<CreateWorkOrderLineItemDTO, WorkOrderLineItem>()
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())

            // Let handler calculate TotalCost
            .ForMember(dest => dest.TotalCost, opt => opt.Ignore())

            // Ignore navigation properties
            .ForMember(dest => dest.WorkOrder, opt => opt.Ignore())
            .ForMember(dest => dest.InventoryItem, opt => opt.Ignore())
            .ForMember(dest => dest.ServiceTask, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());
    }
}