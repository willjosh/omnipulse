using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.ServicePrograms.Command.AddVehicleToServiceProgram;

public sealed class AddVehicleToServiceProgramCommandHandler : IRequestHandler<AddVehicleToServiceProgramCommand, (int ServiceProgramID, int VehicleID)>
{
    private readonly IXrefServiceProgramVehicleRepository _xrefRepository;
    private readonly IServiceProgramRepository _serviceProgramRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IValidator<AddVehicleToServiceProgramCommand> _validator;
    private readonly IAppLogger<AddVehicleToServiceProgramCommandHandler> _logger;
    private readonly IMapper _mapper;

    public AddVehicleToServiceProgramCommandHandler(
        IXrefServiceProgramVehicleRepository xrefRepository,
        IServiceProgramRepository serviceProgramRepository,
        IVehicleRepository vehicleRepository,
        IValidator<AddVehicleToServiceProgramCommand> validator,
        IAppLogger<AddVehicleToServiceProgramCommandHandler> logger,
        IMapper mapper)
    {
        _xrefRepository = xrefRepository;
        _serviceProgramRepository = serviceProgramRepository;
        _vehicleRepository = vehicleRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<(int ServiceProgramID, int VehicleID)> Handle(AddVehicleToServiceProgramCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(AddVehicleToServiceProgramCommand)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Validate ServiceProgram exists and is active
        var serviceProgram = await _serviceProgramRepository.GetByIdAsync(request.ServiceProgramID);
        if (serviceProgram == null || !serviceProgram.IsActive)
        {
            _logger.LogError($"{nameof(ServiceProgram)} {nameof(ServiceProgram.ID)} not found or inactive: {request.ServiceProgramID}");
            throw new EntityNotFoundException(nameof(ServiceProgram), nameof(ServiceProgram.ID), request.ServiceProgramID.ToString());
        }

        // Validate Vehicle exists and is active
        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleID);
        if (vehicle == null || vehicle.Status != Domain.Entities.Enums.VehicleStatusEnum.ACTIVE)
        {
            _logger.LogError($"{nameof(Vehicle)} {nameof(Vehicle.ID)} not found or not active: {request.VehicleID}");
            throw new EntityNotFoundException(nameof(Vehicle), nameof(Vehicle.ID), request.VehicleID.ToString());
        }

        // Check for existing xref (duplicate)
        var xrefExists = await _xrefRepository.ExistsAsync(request.ServiceProgramID, request.VehicleID);
        if (xrefExists)
        {
            _logger.LogError($"{nameof(XrefServiceProgramVehicle)} already exists: {nameof(XrefServiceProgramVehicle.ServiceProgramID)}={request.ServiceProgramID}, {nameof(XrefServiceProgramVehicle.VehicleID)}={request.VehicleID}");
            throw new DuplicateEntityException(nameof(XrefServiceProgramVehicle), $"{nameof(XrefServiceProgramVehicle.ServiceProgramID)}/{nameof(XrefServiceProgramVehicle.VehicleID)}", $"{request.ServiceProgramID}/{request.VehicleID}");
        }

        // Create new xref using AutoMapper
        var newXref = _mapper.Map<XrefServiceProgramVehicle>(request);
        newXref.AddedAt = DateTime.UtcNow;
        newXref.ServiceProgram = serviceProgram;
        newXref.Vehicle = vehicle;
        newXref.VehicleMileageAtAssignment = vehicle.Mileage;

        await _xrefRepository.AddAsync(newXref);
        _logger.LogInformation($"{nameof(XrefServiceProgramVehicle)} created successfully: {nameof(XrefServiceProgramVehicle.ServiceProgramID)}={request.ServiceProgramID}, {nameof(XrefServiceProgramVehicle.VehicleID)}={request.VehicleID}");

        return (request.ServiceProgramID, request.VehicleID);
    }
}