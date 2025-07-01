using System;

namespace Application.MappingProfiles;

using Application.Features.Vehicles.Command.CreateVehicle;
using Application.Features.Vehicles.Command.UpdateVehicle;
using Application.Features.Vehicles.Query.GetAllVehicle;
using Application.Features.Vehicles.Query.GetVehicleDetails;

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

        CreateMap<UpdateVehicleCommand, Vehicle>()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.VehicleID)) // Map the command's VehicleID to the entity's ID
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


        CreateMap<Vehicle, GetVehicleDetailsDTO>()
            .ForMember(dest => dest.VehicleGroupName, opt => opt.MapFrom(
                src => src.VehicleGroup != null ? src.VehicleGroup.Name
                : string.Empty
            ))
            .ForMember(dest => dest.AssignedTechnicianName, opt => opt.MapFrom(
                src => src.User != null ? src.User.FirstName + " " + src.User.LastName
                : "Not Assigned"
            ))
            .ForMember(dest => dest.AssignedTechnicianID, opt => opt.MapFrom(
                src => src.AssignedTechnicianID ?? string.Empty
            ));

        CreateMap<Vehicle, GetAllVehicleDTO>()
            .ForMember(dest => dest.VehicleGroupName, opt => opt.MapFrom(
                src => src.VehicleGroup != null ? src.VehicleGroup.Name
                : string.Empty
            ))
            .ForMember(dest => dest.AssignedTechnicianName, opt => opt.MapFrom(
                src => src.User != null ? src.User.FirstName + " " + src.User.LastName
                : "Not Assigned"
            ))
            .ForMember(dest => dest.AssignedTechnicianID, opt => opt.MapFrom(
                src => src.AssignedTechnicianID ?? string.Empty
            ));
    }
}