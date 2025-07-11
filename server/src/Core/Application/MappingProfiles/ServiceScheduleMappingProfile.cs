using Application.Features.ServiceSchedules.Command.CreateServiceSchedule;
using Application.Features.ServiceSchedules.Command.UpdateServiceSchedule;
using Application.Features.ServiceSchedules.Query.GetAllServiceSchedule;
using Application.Features.ServiceSchedules.Query.GetServiceSchedule;

using AutoMapper;

using Domain.Entities;

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
            .ForMember(dest => dest.XrefServiceScheduleServiceTasks, opt => opt.Ignore())
            .ForMember(dest => dest.ServiceProgram, opt => opt.Ignore());

        // UpdateServiceSchedule
        CreateMap<UpdateServiceScheduleCommand, ServiceSchedule>()
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.XrefServiceScheduleServiceTasks, opt => opt.Ignore())
            .ForMember(dest => dest.ServiceProgram, opt => opt.Ignore());

        // GetServiceSchedule
        CreateMap<ServiceSchedule, GetServiceScheduleDTO>(MemberList.Destination);

        // GetAllServiceSchedule
        CreateMap<ServiceSchedule, GetAllServiceScheduleDTO>(MemberList.Destination)
            .ForMember(dest => dest.ServiceTasks, opt => opt.Ignore());
    }
}