using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;

using MediatR;

namespace Application.Features.Vehicles.Query.GetVehicleStatusData;

public class GetVehicleStatusDataQueryHandler : IRequestHandler<GetVehicleStatusDataQuery, GetVehicleStatusDataDTO>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IAppLogger<GetVehicleStatusDataQueryHandler> _logger;

    public GetVehicleStatusDataQueryHandler(IVehicleRepository vehicleRepository, IAppLogger<GetVehicleStatusDataQueryHandler> logger)
    {
        _vehicleRepository = vehicleRepository;
        _logger = logger;
    }

    public async Task<GetVehicleStatusDataDTO> Handle(GetVehicleStatusDataQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetVehicleStatusDataQuery");
        var activeCount = await _vehicleRepository.GetActiveVehicleCountAsync();
        var maintenanceCount = await _vehicleRepository.GetMaintenanceVehicleCountAsync();
        var outOfServiceCount = await _vehicleRepository.GetOutOfServiceVehicleCountAsync();
        var inactiveCount = await _vehicleRepository.GetInactiveVehicleCountAsync();

        var result = new GetVehicleStatusDataDTO
        {
            ActiveVehicleCount = activeCount,
            MaintenanceVehicleCount = maintenanceCount,
            OutOfServiceVehicleCount = outOfServiceCount,
            InactiveVehicleCount = inactiveCount
        };

        _logger.LogInformation("GetVehicleStatusDataQuery handled successfully");
        return result;
    }
}