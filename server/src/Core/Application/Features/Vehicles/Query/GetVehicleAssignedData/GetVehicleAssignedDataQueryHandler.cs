using Application.Contracts.Logger;
using Application.Contracts.Persistence;

using MediatR;

namespace Application.Features.Vehicles.Query.GetVehicleAssignedData;

public class GetVehicleAssignedDataQueryHandler : IRequestHandler<GetVehicleAssignedDataQuery, GetVehicleAssignedDataDTO>
{
    public readonly IVehicleRepository _vehicleRepository;
    public readonly IAppLogger<GetVehicleAssignedDataQueryHandler> _logger;
    public GetVehicleAssignedDataQueryHandler(IVehicleRepository vehicleRepository, IAppLogger<GetVehicleAssignedDataQueryHandler> logger)
    {
        _vehicleRepository = vehicleRepository;
        _logger = logger;
    }

    public async Task<GetVehicleAssignedDataDTO> Handle(GetVehicleAssignedDataQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetVehicleAssignedDataQuery");

        var assignedVehicleCount = await _vehicleRepository.GetAllAssignedVehicleAsync();
        var unassignedVehicleCount = await _vehicleRepository.GetAllUnassignedVehicleAsync();

        var result = new GetVehicleAssignedDataDTO
        {
            AssignedVehicleCount = assignedVehicleCount,
            UnassignedVehicleCount = unassignedVehicleCount
        };

        _logger.LogInformation("Handled GetVehicleAssignedDataQuery successfully");

        return result;
    }
}