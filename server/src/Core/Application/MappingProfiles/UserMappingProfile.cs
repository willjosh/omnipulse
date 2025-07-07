using System;

using Application.Features.Users.Command.CreateTechnician;
using Application.Features.Users.Command.UpdateTechnician;
using Application.Features.Users.Query.GetAllTechnician;

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

        CreateMap<UpdateTechnicianCommand, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.MaintenanceHistories, opt => opt.Ignore())
            .ForMember(dest => dest.IssueAttachments, opt => opt.Ignore())
            .ForMember(dest => dest.VehicleAssignments, opt => opt.Ignore())
            .ForMember(dest => dest.VehicleDocuments, opt => opt.Ignore())
            .ForMember(dest => dest.VehicleInspections, opt => opt.Ignore())
            .ForMember(dest => dest.Vehicles, opt => opt.Ignore());

        // rest should be automatically mapped by AutoMapper
        CreateMap<User, GetAllTechnicianDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.HireDate, opt => opt.MapFrom(src => src.HireDate))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
    }
}