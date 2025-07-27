using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using MediatR;

namespace Application.Features.VehicleGroups.Command.DeleteVehicleGroup;

public class DeleteVehicleGroupCommandHandler : IRequestHandler<DeleteVehicleGroupCommand, int>
{
    private readonly IVehicleGroupRepository _vehicleGroupRepository;
    private readonly IAppLogger<DeleteVehicleGroupCommandHandler> _logger;
    private readonly IMapper _mapper;

    public DeleteVehicleGroupCommandHandler(IVehicleGroupRepository vehicleGroupRepository, IAppLogger<DeleteVehicleGroupCommandHandler> logger, IMapper mapper)
    {
        _vehicleGroupRepository = vehicleGroupRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> Handle(DeleteVehicleGroupCommand request, CancellationToken cancellationToken)
    {
        // Validate VehicleGroupID
        _logger.LogInformation($"Validating VehicleGroup with ID: {request.VehicleGroupID}");
        var vehicleGroup = await _vehicleGroupRepository.GetByIdAsync(request.VehicleGroupID);
        if (vehicleGroup == null)
        {
            _logger.LogError($"VehicleGroup with ID {request.VehicleGroupID} not found.");
            throw new EntityNotFoundException(typeof(VehicleGroup).ToString(), "ID", request.VehicleGroupID.ToString());
        }

        // Delete VehicleGroup
        _vehicleGroupRepository.Delete(vehicleGroup);

        // Save Changes
        await _vehicleGroupRepository.SaveChangesAsync();
        _logger.LogInformation($"VehicleGroup with ID: {request.VehicleGroupID} deleted");

        return request.VehicleGroupID;
    }
}