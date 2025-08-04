using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Models.PaginationModels;

using AutoMapper;

using FluentValidation;

using MediatR;

namespace Application.Features.InspectionForms.Query.GetAllInspectionForm;

public class GetAllInspectionFormQueryHandler : IRequestHandler<GetAllInspectionFormQuery, PagedResult<InspectionFormDTO>>
{
    private readonly IInspectionFormRepository _inspectionFormRepository;
    private readonly IValidator<GetAllInspectionFormQuery> _validator;
    private readonly IAppLogger<GetAllInspectionFormQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetAllInspectionFormQueryHandler(
        IInspectionFormRepository inspectionFormRepository,
        IValidator<GetAllInspectionFormQuery> validator,
        IAppLogger<GetAllInspectionFormQueryHandler> logger,
        IMapper mapper)
    {
        _inspectionFormRepository = inspectionFormRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<PagedResult<InspectionFormDTO>> Handle(GetAllInspectionFormQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)}() - Handling {nameof(GetAllInspectionFormQuery)}({nameof(GetAllInspectionFormQuery.Parameters)}: {request.Parameters})");

        // Validate request
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(GetAllInspectionFormQueryValidator)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Get all inspection forms from the repository
        var result = await _inspectionFormRepository.GetAllInspectionFormsPagedAsync(request.Parameters);

        // Filter to only include active inspection forms
        var activeInspectionForms = result.Items.Where(x => x != null && x.IsActive).ToList();

        // Map the active inspection forms to DTOs
        var inspectionFormDTOs = _mapper.Map<List<InspectionFormDTO>>(activeInspectionForms);

        // Set calculated properties for each DTO
        foreach (var dto in inspectionFormDTOs)
        {
            var inspectionForm = activeInspectionForms.FirstOrDefault(x => x.ID == dto.ID);
            if (inspectionForm != null)
            {
                dto.InspectionCount = inspectionForm.Inspections?.Count ?? 0;
                dto.InspectionFormItemCount = inspectionForm.InspectionFormItems?.Where(item => item != null && item.IsActive).Count() ?? 0;
            }
        }

        var pagedResult = new PagedResult<InspectionFormDTO>
        {
            Items = inspectionFormDTOs,
            TotalCount = activeInspectionForms.Count,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        _logger.LogInformation($"Returning {pagedResult.TotalCount} inspection forms for page {pagedResult.PageNumber} with page size {pagedResult.PageSize}");
        return pagedResult;
    }
}