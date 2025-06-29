using Application.Contracts.Persistence;
using Application.Contracts.Logger;
using MediatR;
using AutoMapper;
using Application.Exceptions;

namespace Application.Features.Vehicles.Command.DeactivateVehicle;

public class DeactivateVehicleCommandHandler : IRequestHandler<DeactivateVehicleCommand, int>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IAppLogger<DeactivateVehicleCommandHandler> _logger;
    private readonly IMapper _mapper;

    public DeactivateVehicleCommandHandler(IVehicleRepository vehicleRepository, IAppLogger<DeactivateVehicleCommandHandler> logger, IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> Handle(DeactivateVehicleCommand request, CancellationToken cancellationToken)
    {
        // Validate Request
        if (request.VehicleID <= 0)
        {
            throw new BadRequestException("VehicleID must be greater than 0");
        }

        // Validate VehicleID
        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleID) ?? throw new EntityNotFoundException("Vehicle", "ID", request.VehicleID.ToString());
        if (vehicle.Status == Domain.Entities.Enums.VehicleStatusEnum.INACTIVE)
        {
            throw new BadRequestException("Vehicle is already deactivated");
        }

        // Deactivate Vehicle
        await _vehicleRepository.VehicleDeactivateAsync(request.VehicleID);

        // Save Changes
        await _vehicleRepository.SaveChangesAsync();

        // Return VehicleID
        return request.VehicleID;
    }
}
