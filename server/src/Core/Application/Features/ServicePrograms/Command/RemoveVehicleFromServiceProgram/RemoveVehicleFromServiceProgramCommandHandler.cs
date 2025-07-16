using System.Threading;
using System.Threading.Tasks;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using FluentValidation;

using MediatR;

namespace Application.Features.ServicePrograms.Command.RemoveVehicleFromServiceProgram;

public sealed class RemoveVehicleFromServiceProgramCommandHandler : IRequestHandler<RemoveVehicleFromServiceProgramCommand, (int ServiceProgramID, int VehicleID)>
{
    private readonly IXrefServiceProgramVehicleRepository _xrefRepository;
    private readonly IServiceProgramRepository _serviceProgramRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IValidator<RemoveVehicleFromServiceProgramCommand> _validator;
    private readonly IAppLogger<RemoveVehicleFromServiceProgramCommandHandler> _logger;

    public RemoveVehicleFromServiceProgramCommandHandler(
        IXrefServiceProgramVehicleRepository xrefRepository,
        IServiceProgramRepository serviceProgramRepository,
        IVehicleRepository vehicleRepository,
        IValidator<RemoveVehicleFromServiceProgramCommand> validator,
        IAppLogger<RemoveVehicleFromServiceProgramCommandHandler> logger)
    {
        _xrefRepository = xrefRepository;
        _serviceProgramRepository = serviceProgramRepository;
        _vehicleRepository = vehicleRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<(int ServiceProgramID, int VehicleID)> Handle(RemoveVehicleFromServiceProgramCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(RemoveVehicleFromServiceProgramCommand)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Check ServiceProgram exists
        var serviceProgram = await _serviceProgramRepository.GetByIdAsync(request.ServiceProgramID);
        if (serviceProgram == null)
        {
            _logger.LogError($"ServiceProgram not found: {request.ServiceProgramID}");
            throw new EntityNotFoundException("ServiceProgram", "ID", request.ServiceProgramID.ToString());
        }

        // Check Vehicle exists
        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleID);
        if (vehicle == null)
        {
            _logger.LogError($"Vehicle not found: {request.VehicleID}");
            throw new EntityNotFoundException("Vehicle", "ID", request.VehicleID.ToString());
        }

        // Check xref exists
        var exists = await _xrefRepository.ExistsAsync(request.ServiceProgramID, request.VehicleID);
        if (!exists)
        {
            _logger.LogError($"XrefServiceProgramVehicle not found: ServiceProgramID={request.ServiceProgramID}, VehicleID={request.VehicleID}");
            throw new EntityNotFoundException("XrefServiceProgramVehicle", "ServiceProgramID/VehicleID", $"{request.ServiceProgramID}/{request.VehicleID}");
        }

        // Remove xref
        await _xrefRepository.RemoveAsync(request.ServiceProgramID, request.VehicleID);
        _logger.LogInformation($"XrefServiceProgramVehicle removed: ServiceProgramID={request.ServiceProgramID}, VehicleID={request.VehicleID}");

        return (request.ServiceProgramID, request.VehicleID);
    }
}