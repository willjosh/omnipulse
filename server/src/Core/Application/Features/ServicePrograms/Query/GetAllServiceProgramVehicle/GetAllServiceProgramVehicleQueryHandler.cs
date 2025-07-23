using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Models.PaginationModels;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.ServicePrograms.Query.GetAllServiceProgramVehicle;

public class GetAllServiceProgramVehicleQueryHandler : IRequestHandler<GetAllServiceProgramVehicleQuery, PagedResult<XrefServiceProgramVehicleDTO>>
{
    private readonly IServiceProgramRepository _serviceProgramRepository;
    private readonly IXrefServiceProgramVehicleRepository _xrefServiceProgramVehicleRepository;
    private readonly IValidator<GetAllServiceProgramVehicleQuery> _validator;
    private readonly IAppLogger<GetAllServiceProgramVehicleQueryHandler> _logger;

    public GetAllServiceProgramVehicleQueryHandler(
        IServiceProgramRepository serviceProgramRepository,
        IXrefServiceProgramVehicleRepository xrefServiceProgramVehicleRepository,
        IValidator<GetAllServiceProgramVehicleQuery> validator,
        IAppLogger<GetAllServiceProgramVehicleQueryHandler> logger)
    {
        _serviceProgramRepository = serviceProgramRepository;
        _xrefServiceProgramVehicleRepository = xrefServiceProgramVehicleRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<PagedResult<XrefServiceProgramVehicleDTO>> Handle(GetAllServiceProgramVehicleQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Parameters);

        _logger.LogInformation($"Handling {nameof(GetAllServiceProgramVehicleQuery)} for {nameof(request.ServiceProgramID)}: {request.ServiceProgramID}");

        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(GetAllServiceProgramVehicleQuery)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Verify service program exists
        var serviceProgram = await _serviceProgramRepository.GetByIdAsync(request.ServiceProgramID);
        if (serviceProgram == null)
        {
            _logger.LogError($"{nameof(ServiceProgram)} with {nameof(request.ServiceProgramID)} {request.ServiceProgramID} not found.");
            throw new EntityNotFoundException(nameof(ServiceProgram), nameof(request.ServiceProgramID), request.ServiceProgramID.ToString());
        }

        // TODO: Check if service program is active
        // if (!serviceProgram.IsActive)
        // {
        //     var errorMessage = $"{nameof(ServiceProgram)} with {nameof(request.ServiceProgramID)} {request.ServiceProgramID} is not active.";
        //     _logger.LogWarning($"{nameof(GetAllServiceProgramVehicleQuery)} - Attempted to access inactive {nameof(ServiceProgram)} with {nameof(request.ServiceProgramID)} {request.ServiceProgramID}");
        //     throw new BadRequestException(errorMessage);
        // }

        // Get paged xref records with vehicle navigation properties from repository (domain entities only)
        var xrefResult = await _xrefServiceProgramVehicleRepository.GetAllByServiceProgramIDPagedAsync(request.ServiceProgramID, request.Parameters);

        // Transform domain entities to DTOs
        var dtoItems = xrefResult.Items.Select(xref => new XrefServiceProgramVehicleDTO
        {
            ServiceProgramID = xref.ServiceProgramID,
            VehicleID = xref.Vehicle.ID,
            VehicleName = xref.Vehicle.Name,
            AddedAt = xref.AddedAt
        }).ToList();

        var result = new PagedResult<XrefServiceProgramVehicleDTO>
        {
            Items = dtoItems,
            TotalCount = xrefResult.TotalCount,
            PageNumber = xrefResult.PageNumber,
            PageSize = xrefResult.PageSize
        };

        _logger.LogInformation($"Returning {result.Items.Count} vehicles for {nameof(ServiceProgram)} with {nameof(request.ServiceProgramID)} {request.ServiceProgramID}, page {request.Parameters.PageNumber}");

        return result;
    }
}