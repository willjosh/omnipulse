using System;

using Application.Features.Users.Command.CreateTechnician;

using AutoMapper;

using Domain.Entities;

namespace Application.MappingProfiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<CreateTechnicianCommand, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.MaintenanceHistories, opt => opt.Ignore())
            .ForMember(dest => dest.IssueAttachments, opt => opt.Ignore())
            .ForMember(dest => dest.VehicleAssignments, opt => opt.Ignore())
            .ForMember(dest => dest.VehicleDocuments, opt => opt.Ignore())
            .ForMember(dest => dest.VehicleInspections, opt => opt.Ignore())
            .ForMember(dest => dest.Vehicles, opt => opt.Ignore());
    }
}