using Application.Features.MaintenanceHistories.Command.CreateMaintenanceHistory;
using Application.Features.MaintenanceHistories.Query.GetAllMaintenanceHistories;

using AutoMapper;

using Domain.Entities;

namespace Application.MappingProfiles;

public class MaintenanceHistoryMappingProfile : Profile
{
    public MaintenanceHistoryMappingProfile()
    {
        CreateMap<CreateMaintenanceHistoryCommand, MaintenanceHistory>()
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ForMember(dest => dest.WorkOrder, opt => opt.Ignore())
            .ForMember(dest => dest.InventoryTransactions, opt => opt.Ignore());

        CreateMap<MaintenanceHistory, GetAllMaintenanceHistoryDTO>()
            .ForMember(dest => dest.MaintenanceHistoryID, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.WorkOrderNumber, opt => opt.MapFrom(src => src.WorkOrder != null ? src.WorkOrder.Title : string.Empty));

        CreateMap<WorkOrder, MaintenanceHistory>()
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.MileageAtService, opt => opt.MapFrom(src => src.EndOdometer ?? src.StartOdometer))
            .ForMember(dest => dest.ServiceDate, opt => opt.MapFrom(src => src.ActualStartDate ?? DateTime.Now))
            .ForMember(dest => dest.WorkOrderID, opt => opt.MapFrom(src => src.ID));
    }
}