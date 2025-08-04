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
            .ForMember(dest => dest.WorkOrder, opt => opt.Ignore());

        CreateMap<MaintenanceHistory, GetAllMaintenanceHistoryDTO>()
            .ForMember(dest => dest.MaintenanceHistoryID, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.VehicleID, opt => opt.MapFrom(src => src.WorkOrder.VehicleID))
            .ForMember(dest => dest.VehicleName, opt => opt.MapFrom(src => src.WorkOrder.Vehicle.Name))
            .ForMember(dest => dest.ServiceTaskName, opt => opt.MapFrom(src => src.WorkOrder.WorkOrderLineItems.Select(wol => wol.ServiceTask.Name).ToList()))
            .ForMember(dest => dest.ServiceTaskID, opt => opt.MapFrom(src => src.WorkOrder.WorkOrderLineItems.Select(wol => wol.ServiceTask.ID).ToArray()))
            .ForMember(dest => dest.TechnicianID, opt => opt.MapFrom(src => src.WorkOrder.AssignedToUserID))
            .ForMember(dest => dest.TechnicianName, opt => opt.MapFrom(src => src.WorkOrder.User.FirstName + " " + src.WorkOrder.User.LastName))
            .ForMember(dest => dest.ServiceDate, opt => opt.MapFrom(src => src.WorkOrder.ActualCompletionDate))
            .ForMember(dest => dest.Cost, opt => opt.Ignore())
            .ForMember(dest => dest.LabourHours, opt => opt.Ignore());

        CreateMap<WorkOrder, MaintenanceHistory>()
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.MileageAtService, opt => opt.MapFrom(src => src.EndOdometer ?? src.StartOdometer))
            .ForMember(dest => dest.ServiceDate, opt => opt.MapFrom(src => src.ActualStartDate ?? DateTime.Now))
            .ForMember(dest => dest.WorkOrderID, opt => opt.MapFrom(src => src.ID));
    }
}