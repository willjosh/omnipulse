using System;

namespace Application.MappingProfiles;

using Application.Features.Vehicle.Command.CreateVehicle;
using AutoMapper;
using Domain.Entities;

public class VehicleMappingProfile : Profile
{
    public VehicleMappingProfile()
    {
        CreateMap<CreateVehicleCommand, Vehicle>()
            .ForMember(dest => dest.ID, opt => opt.Ignore()) // Don't map ID - will be auto-generated
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Handled by BaseEntity
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // Handled by BaseEntity
            .ForMember(dest => dest.VehicleGroup, opt => opt.Ignore()) // Navigation property
            .ForMember(dest => dest.VehicleImages, opt => opt.Ignore()) // Navigation collection
            .ForMember(dest => dest.VehicleAssignments, opt => opt.Ignore()) // Navigation collection
            .ForMember(dest => dest.VehicleDocuments, opt => opt.Ignore()) // Navigation collection
            .ForMember(dest => dest.VehicleServicePrograms, opt => opt.Ignore()) // Navigation collection
            .ForMember(dest => dest.ServiceReminders, opt => opt.Ignore()) // Navigation collection
            .ForMember(dest => dest.Issues, opt => opt.Ignore()) // Navigation collection
            .ForMember(dest => dest.VehicleInspections, opt => opt.Ignore()); // Navigation collection
    }
}
