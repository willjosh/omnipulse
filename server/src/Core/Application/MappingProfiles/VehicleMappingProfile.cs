using System;

namespace Application.MappingProfiles;

using Application.Features.Vehicle.Command.CreateVehicle;
using AutoMapper;
using Domain.Entities;

public class VehicleMappingProfile : Profile
{
    public VehicleMappingProfile()
    {
        CreateMap<CreateVehicleCommand, Vehicle>();
    }
}
