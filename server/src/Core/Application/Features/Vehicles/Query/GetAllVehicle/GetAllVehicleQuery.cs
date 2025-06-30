using System;
using MediatR;

namespace Application.Features.Vehicles.Query.GetAllVehicle;

public record GetAllVehicleQuery : IRequest<IReadOnlyList<GetAllVehicleDTO>> { }
