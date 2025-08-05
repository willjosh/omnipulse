using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using MediatR;

namespace Application.Features.InspectionForms.Query.GetInspectionForm;

public sealed class GetInspectionFormQueryHandler : IRequestHandler<GetInspectionFormQuery, InspectionFormDTO>
{
    private readonly IInspectionFormRepository _inspectionFormRepository;
    private readonly IAppLogger<GetInspectionFormQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetInspectionFormQueryHandler(
        IInspectionFormRepository inspectionFormRepository,
        IAppLogger<GetInspectionFormQueryHandler> logger,
        IMapper mapper)
    {
        _inspectionFormRepository = inspectionFormRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<InspectionFormDTO> Handle(GetInspectionFormQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)} - Handling {nameof(GetInspectionFormQuery)}({request.InspectionFormID})");

        // Get inspection form with related data
        var inspectionForm = await _inspectionFormRepository.GetInspectionFormWithItemsAsync(request.InspectionFormID);

        // Check if inspection form exists or is inactive
        if (inspectionForm == null || !inspectionForm.IsActive)
        {
            _logger.LogError($"Inspection form with ID {request.InspectionFormID} not found or is inactive.");
            throw new EntityNotFoundException(nameof(InspectionForm), nameof(InspectionForm.ID), request.InspectionFormID.ToString());
        }

        // Map to DTO
        var inspectionFormDto = _mapper.Map<InspectionFormDTO>(inspectionForm);

        // Set additional calculated properties (only count active items)
        inspectionFormDto.InspectionCount = inspectionForm.Inspections?.Count ?? 0;
        inspectionFormDto.InspectionFormItemCount = inspectionForm.InspectionFormItems?.Where(item => item != null && item.IsActive).Count() ?? 0;

        _logger.LogInformation($"Returning InspectionFormDTO for InspectionFormID: {request.InspectionFormID}");
        return inspectionFormDto;
    }
}