using MediatR;

namespace Application.Features.Vehicles.Query.GetVehicleStatusData;

public record GetVehicleStatusDataQuery : IRequest<GetVehicleStatusDataDTO> { }