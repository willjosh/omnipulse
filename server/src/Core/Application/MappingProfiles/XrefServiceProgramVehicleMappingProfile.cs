using Application.Features.ServicePrograms.Command.AddVehicleToServiceProgram;

using AutoMapper;

using Domain.Entities;

namespace Application.MappingProfiles;

public class XrefServiceProgramVehicleMappingProfile : Profile
{
    public XrefServiceProgramVehicleMappingProfile()
    {
        // AddVehicleToServiceProgram
        CreateMap<AddVehicleToServiceProgramCommand, XrefServiceProgramVehicle>(MemberList.Destination)
            .ForMember(dest => dest.ServiceProgramID, opt => opt.MapFrom(src => src.ServiceProgramID))
            .ForMember(dest => dest.VehicleID, opt => opt.MapFrom(src => src.VehicleID))
            .ForMember(dest => dest.AddedAt, opt => opt.Ignore()) // Set by AddVehicleToServiceProgramCommandHandler
            .ForMember(dest => dest.ServiceProgram, opt => opt.Ignore()) // Navigation Property
            .ForMember(dest => dest.Vehicle, opt => opt.Ignore()) // Navigation Property
            .ForMember(dest => dest.User, opt => opt.Ignore()); // Navigation Property
    }
}