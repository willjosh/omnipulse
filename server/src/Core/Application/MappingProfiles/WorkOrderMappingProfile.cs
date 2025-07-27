using Application.Features.WorkOrders.Command.CreateWorkOrder;
using Application.Features.WorkOrders.Query.GetWorkOrderDetail;

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
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.MaintenanceHistories, opt => opt.Ignore())
            .ForMember(dest => dest.WorkOrderLineItems, opt => opt.Ignore())
            .ForMember(dest => dest.Invoices, opt => opt.Ignore())
            .ForMember(dest => dest.InventoryTransactions, opt => opt.Ignore());

        CreateMap<WorkOrder, GetWorkOrderDetailDTO>()
            // Vehicle mappings
            .ForMember(dest => dest.VehicleName, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Name : "Unknown Vehicle"))

            // User mappings
            .ForMember(dest => dest.AssignedToUserName, opt => opt.MapFrom(src => src.User != null ? src.User.GetFullName() : "Unassigned"))

            // Line items mapping
            .ForMember(dest => dest.WorkOrderLineItems, opt => opt.Ignore())

            // Cost calculations - ignore in mapping, calculate in handler
            .ForMember(dest => dest.TotalCost, opt => opt.Ignore())
            .ForMember(dest => dest.TotalLaborCost, opt => opt.Ignore())
            .ForMember(dest => dest.TotalItemCost, opt => opt.Ignore());
    }
}