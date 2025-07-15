using Application.Features.ServicePrograms.Command.CreateServiceProgram;

using AutoMapper;

using Domain.Entities;

namespace Application.MappingProfiles;

public class ServiceProgramMappingProfile : Profile
{
    public ServiceProgramMappingProfile()
    {
        // CreateServiceProgram
        CreateMap<CreateServiceProgramCommand, ServiceProgram>()
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ServiceSchedules, opt => opt.Ignore()) // Navigation Property
            .ForMember(dest => dest.XrefServiceProgramVehicles, opt => opt.Ignore()); // Navigation Property
    }
}