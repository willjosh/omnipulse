using System;
using Application.Features.VehicleGroups.Command.CreateVehicleGroup;
using Application.Features.VehicleGroups.Command.UpdateVehicleGroup;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles;

public class VehicleGroupMappingProfile : Profile
{
    public VehicleGroupMappingProfile()
    {
        CreateMap<CreateVehicleGroupCommand, VehicleGroup>()
            .ForMember(dest => dest.ID, opt => opt.Ignore()) // Don't map ID - will be auto-generated
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Handled by BaseEntity
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()); // Handled by BaseEntity

        CreateMap<UpdateVehicleGroupCommand, VehicleGroup>()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.VehicleGroupId)) // Map the command's VehicleGroupId to the entity's ID
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Handled by BaseEntity
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()); // Handled by BaseEntity
    }
}
