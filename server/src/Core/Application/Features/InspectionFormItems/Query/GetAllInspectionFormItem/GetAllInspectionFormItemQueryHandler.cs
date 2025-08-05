using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Models.PaginationModels;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.InspectionFormItems.Query.GetAllInspectionFormItem;

/// <summary>
/// Handles <see cref="GetAllInspectionFormItemQuery"/>
/// </summary>
public sealed class GetAllInspectionFormItemQueryHandler : IRequestHandler<GetAllInspectionFormItemQuery, PagedResult<InspectionFormItemDetailDTO>>
{
    private readonly IInspectionFormItemRepository _inspectionFormItemRepository;
    private readonly IInspectionFormRepository _inspectionFormRepository;
    private readonly IValidator<GetAllInspectionFormItemQuery> _validator;
    private readonly IAppLogger<GetAllInspectionFormItemQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetAllInspectionFormItemQueryHandler(
        IInspectionFormItemRepository inspectionFormItemRepository,
        IInspectionFormRepository inspectionFormRepository,
        IValidator<GetAllInspectionFormItemQuery> validator,
        IAppLogger<GetAllInspectionFormItemQueryHandler> logger,
        IMapper mapper)
    {
        _inspectionFormItemRepository = inspectionFormItemRepository;
        _inspectionFormRepository = inspectionFormRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<PagedResult<InspectionFormItemDetailDTO>> Handle(GetAllInspectionFormItemQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)} - Handling InspectionFormID: {request.InspectionFormID}");

        // Validate request
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(Handle)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Validate that the inspection form exists and is active
        var inspectionForm = await _inspectionFormRepository.GetByIdAsync(request.InspectionFormID);
        if (inspectionForm == null || !inspectionForm.IsActive)
        {
            _logger.LogError($"Inspection form with ID {request.InspectionFormID} not found or is inactive.");
            throw new EntityNotFoundException(nameof(InspectionForm), nameof(InspectionForm.ID), request.InspectionFormID.ToString());
        }

        // Get paginated inspection form items for the specified form
        var result = await _inspectionFormItemRepository.GetAllByInspectionFormIdPagedAsync(request.InspectionFormID, request.Parameters);

        // Filter to only include active inspection form items
        var activeInspectionFormItems = result.Items.Where(x => x != null && x.IsActive).ToList();

        // Map to DTOs
        var inspectionFormItemDTOs = _mapper.Map<List<InspectionFormItemDetailDTO>>(activeInspectionFormItems);

        var pagedResult = new PagedResult<InspectionFormItemDetailDTO>
        {
            Items = inspectionFormItemDTOs,
            TotalCount = activeInspectionFormItems.Count,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        _logger.LogInformation($"Returning {pagedResult.TotalCount} inspection form items (page {pagedResult.PageNumber}) for InspectionFormID: {request.InspectionFormID}");
        return pagedResult;
    }
}