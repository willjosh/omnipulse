using Application.Features.ServicePrograms.Command.CreateServiceProgram;
using Application.Features.ServicePrograms.Command.UpdateServiceProgram;
using Application.Features.ServicePrograms.Query;
using Application.Features.ServicePrograms.Query.GetAllServiceProgram;

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

        // UpdateServiceProgram
        CreateMap<UpdateServiceProgramCommand, ServiceProgram>()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ServiceProgramID)) // Map the command's ServiceProgramID to the entity's ID
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ServiceSchedules, opt => opt.Ignore()) // Navigation Property
            .ForMember(dest => dest.XrefServiceProgramVehicles, opt => opt.Ignore()); // Navigation Property

        // GetServiceProgram
        CreateMap<ServiceProgram, ServiceProgramDTO>(MemberList.Destination)
            .ForMember(dest => dest.ServiceSchedules, opt => opt.Ignore()) // Handled in query handlers
            .ForMember(dest => dest.AssignedVehicleIDs, opt => opt.Ignore()); // Handled in query handlers

        // GetAllServiceProgram
        CreateMap<ServiceProgram, GetAllServiceProgramDTO>()
            .ForMember(dest => dest.ServiceScheduleCount, opt => opt.MapFrom(src => src.ServiceSchedules.Count))
            .ForMember(dest => dest.AssignedVehicleCount, opt => opt.MapFrom(src => src.XrefServiceProgramVehicles.Count));
    }
}