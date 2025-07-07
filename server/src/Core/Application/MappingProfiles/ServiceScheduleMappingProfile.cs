using Application.Features.ServiceSchedules.Command.CreateServiceSchedule;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles;

public class ServiceScheduleMappingProfile : Profile
{
    public ServiceScheduleMappingProfile()
    {
        CreateMap<CreateServiceScheduleCommand, ServiceSchedule>()
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ServiceScheduleTasks, opt => opt.Ignore())
            .ForMember(dest => dest.ServiceProgram, opt => opt.Ignore());
    }
}