using System;
using MediatR;

namespace Application.Features.Vehicles.Query.GetVehicleDetails;

public record GetVehicleDetailsQuery(int VehicleID) : IRequest<GetVehicleDetailsDTO> { }
