using Application.Features.ServicePrograms.Command.AddVehicleToServiceProgram;
using Application.Features.ServicePrograms.Query.GetAllServiceProgramVehicle;

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
            .ForMember(dest => dest.Vehicle, opt => opt.Ignore()); // Navigation Property
                                                                   // .ForMember(dest => dest.User, opt => opt.Ignore()); // Navigation Property // TODO XrefServiceProgramVehicle User

        // GetAllServiceProgramVehicle
        CreateMap<XrefServiceProgramVehicle, XrefServiceProgramVehicleDTO>(MemberList.Destination)
            .ForMember(dest => dest.ServiceProgramID, opt => opt.MapFrom(src => src.ServiceProgramID))
            .ForMember(dest => dest.VehicleID, opt => opt.MapFrom(src => src.VehicleID))
            .ForMember(dest => dest.VehicleName, opt => opt.Ignore()) // Set by GetAllServiceProgramVehicleQueryHandler
            .ForMember(dest => dest.AddedAt, opt => opt.MapFrom(src => src.AddedAt))
            .ForSourceMember(src => src.ServiceProgram, opt => opt.DoNotValidate()) // Navigation Property
            .ForSourceMember(src => src.Vehicle, opt => opt.DoNotValidate()); // Navigation Property
                                                                              // .ForSourceMember(src => src.User, opt => opt.DoNotValidate()); // Navigation Property // TODO XrefServiceProgramVehicle User
    }
}