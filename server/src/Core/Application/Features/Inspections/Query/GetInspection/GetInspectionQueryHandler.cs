using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using MediatR;

namespace Application.Features.Inspections.Query.GetInspection;

public sealed class GetInspectionQueryHandler : IRequestHandler<GetInspectionQuery, InspectionDTO>
{
    private readonly IInspectionRepository _inspectionRepository;
    private readonly IAppLogger<GetInspectionQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetInspectionQueryHandler(
        IInspectionRepository inspectionRepository,
        IAppLogger<GetInspectionQueryHandler> logger,
        IMapper mapper)
    {
        _inspectionRepository = inspectionRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<InspectionDTO> Handle(GetInspectionQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)} - Handling {nameof(GetInspectionQuery)}({request.InspectionID})");

        // Validate input
        if (request.InspectionID <= 0)
        {
            var errorMessage = $"Invalid inspection ID: {request.InspectionID}";
            _logger.LogError(errorMessage);
            throw new BadRequestException(errorMessage);
        }

        // Get inspection with all related data
        var inspection = await _inspectionRepository.GetInspectionWithDetailsAsync(request.InspectionID);

        // Check if inspection exists
        if (inspection == null)
        {
            _logger.LogError($"Inspection with ID {request.InspectionID} not found.");
            throw new EntityNotFoundException(typeof(Inspection).ToString(), nameof(Inspection.ID), request.InspectionID.ToString());
        }

        // Map to DTO
        var inspectionDto = _mapper.Map<InspectionDTO>(inspection);

        _logger.LogInformation($"Returning InspectionDTO for InspectionID: {request.InspectionID}");
        return inspectionDto;
    }
}