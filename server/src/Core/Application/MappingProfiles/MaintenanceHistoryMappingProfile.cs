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
            .ForMember(dest => dest.Vehicle, opt => opt.Ignore())
            .ForMember(dest => dest.WorkOrder, opt => opt.Ignore())
            .ForMember(dest => dest.ServiceTask, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.InventoryTransactions, opt => opt.Ignore());

        CreateMap<MaintenanceHistory, GetAllMaintenanceHistoryDTO>()
            .ForMember(dest => dest.MaintenanceHistoryID, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.VehicleName, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Name : string.Empty))
            .ForMember(dest => dest.WorkOrderNumber, opt => opt.MapFrom(src => src.WorkOrder != null ? src.WorkOrder.Title : string.Empty))
            .ForMember(dest => dest.ServiceTaskName, opt => opt.MapFrom(src => src.ServiceTask != null ? src.ServiceTask.Name : string.Empty))
            .ForMember(dest => dest.TechnicianName, opt => opt.MapFrom(src => src.User != null ? src.User.FirstName + " " + src.User.LastName : string.Empty));
    }
}