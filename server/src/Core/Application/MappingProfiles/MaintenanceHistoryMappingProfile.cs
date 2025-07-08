using Application.Features.MaintenanceHistories.Command.CreateMaintenanceHistory;
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
    }
} 