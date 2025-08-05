using System;

using MediatR;

namespace Application.Features.Vehicles.Query.GetVehicleAssignedData;

public record GetVehicleAssignedDataQuery() : IRequest<GetVehicleAssignedDataDTO> { }