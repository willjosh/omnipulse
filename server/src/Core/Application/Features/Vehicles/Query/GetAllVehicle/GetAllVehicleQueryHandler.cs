using System;
using MediatR;

namespace Application.Features.Vehicles.Query.GetAllVehicle;

public class GetAllVehicleQueryHandler : IRequestHandler<GetAllVehicleQuery, IReadOnlyList<GetAllVehicleDTO>>
{
    public Task<IReadOnlyList<GetAllVehicleDTO>> Handle(GetAllVehicleQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
