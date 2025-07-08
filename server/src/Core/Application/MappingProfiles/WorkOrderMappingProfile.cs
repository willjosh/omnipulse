using System;

using Application.Features.WorkOrders.Command.CreateWorkOrder;

using AutoMapper;

using Domain.Entities;

namespace Application.MappingProfiles;

public class WorkOrderMappingProfile : Profile
{
    public WorkOrderMappingProfile()
    {
        CreateMap<CreateWorkOrderCommand, WorkOrder>()
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Vehicle, opt => opt.Ignore())
            .ForMember(dest => dest.ServiceReminder, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.MaintenanceHistories, opt => opt.Ignore())
            .ForMember(dest => dest.WorkOrderLineItems, opt => opt.Ignore())
            .ForMember(dest => dest.Invoices, opt => opt.Ignore())
            .ForMember(dest => dest.InventoryTransactions, opt => opt.Ignore());
    }
}