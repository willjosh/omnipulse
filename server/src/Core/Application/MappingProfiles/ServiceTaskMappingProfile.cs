using Application.Features.ServiceTasks.Command.CreateServiceTask;
using Application.Features.ServiceTasks.Command.UpdateServiceTask;
using Application.Features.ServiceTasks.Query;

using AutoMapper;

using Domain.Entities;

namespace Application.MappingProfiles;

public class ServiceTaskMappingProfile : Profile
{
    public ServiceTaskMappingProfile()
    {
        // CreateServiceTask
        CreateMap<CreateServiceTaskCommand, ServiceTask>()
            .ForMember(dest => dest.ID, opt => opt.Ignore()) // Don't map ID - will be auto-generated
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Handled by BaseEntity
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // Handled by BaseEntity
            .ForMember(dest => dest.XrefServiceScheduleServiceTasks, opt => opt.Ignore()) // Navigation Collection
            .ForMember(dest => dest.MaintenanceHistories, opt => opt.Ignore()) // Navigation Collection
            .ForMember(dest => dest.WorkOrderLineItems, opt => opt.Ignore()); // Navigation Collection

        // UpdateServiceTask
        CreateMap<UpdateServiceTaskCommand, ServiceTask>()
            .ForMember(dest => dest.ID, opt => opt.Ignore()) // Don't update ID
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Don't update CreatedAt
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // Will be handled by BaseEntity/Context
            .ForMember(dest => dest.XrefServiceScheduleServiceTasks, opt => opt.Ignore()) // Navigation Collection
            .ForMember(dest => dest.MaintenanceHistories, opt => opt.Ignore()) // Navigation Collection
            .ForMember(dest => dest.WorkOrderLineItems, opt => opt.Ignore()); // Navigation Collection

        // GetServiceTask & GetAllServiceTask
        CreateMap<ServiceTask, ServiceTaskDTO>();
    }
}