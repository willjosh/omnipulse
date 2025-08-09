using Application.Features.ServiceSchedules.Command.CreateServiceSchedule;
using Application.Features.ServiceSchedules.Command.UpdateServiceSchedule;
using Application.Features.ServiceSchedules.Query;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

namespace Application.MappingProfiles;

public class ServiceScheduleMappingProfile : Profile
{
    public ServiceScheduleMappingProfile()
    {
        // CreateServiceSchedule
        CreateMap<CreateServiceScheduleCommand, ServiceSchedule>()
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsSoftDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.XrefServiceScheduleServiceTasks, opt => opt.Ignore())
            .ForMember(dest => dest.ServiceProgram, opt => opt.Ignore());

        // UpdateServiceSchedule
        CreateMap<UpdateServiceScheduleCommand, ServiceSchedule>()
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsSoftDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.XrefServiceScheduleServiceTasks, opt => opt.Ignore())
            .ForMember(dest => dest.ServiceProgram, opt => opt.Ignore());

        // GetServiceSchedule & GetAllServiceSchedule
        CreateMap<ServiceSchedule, ServiceScheduleDTO>(MemberList.Destination)
            .ForMember(dest => dest.ServiceTasks, opt => opt.Ignore())
            .ForMember(dest => dest.ScheduleType, opt => opt.MapFrom(src => src.GetScheduleType()));
    }
}