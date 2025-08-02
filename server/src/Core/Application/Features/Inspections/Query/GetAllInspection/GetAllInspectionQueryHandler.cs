using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Models.PaginationModels;

using AutoMapper;

using FluentValidation;

using MediatR;

namespace Application.Features.Inspections.Query.GetAllInspection;

public class GetAllInspectionQueryHandler : IRequestHandler<GetAllInspectionQuery, PagedResult<GetAllInspectionDTO>>
{
    private readonly IInspectionRepository _inspectionRepository;
    private readonly IValidator<GetAllInspectionQuery> _validator;
    private readonly IAppLogger<GetAllInspectionQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetAllInspectionQueryHandler(
        IInspectionRepository inspectionRepository,
        IValidator<GetAllInspectionQuery> validator,
        IAppLogger<GetAllInspectionQueryHandler> logger,
        IMapper mapper)
    {
        _inspectionRepository = inspectionRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<PagedResult<GetAllInspectionDTO>> Handle(GetAllInspectionQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)} - Handling {nameof(GetAllInspectionQuery)}");

        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(GetAllInspectionQuery)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Get paginated inspections
        var pagedInspections = await _inspectionRepository.GetAllInspectionsPagedAsync(request.Parameters);

        // Map to DTOs
        var inspectionDtos = _mapper.Map<List<GetAllInspectionDTO>>(pagedInspections.Items);

        var result = new PagedResult<GetAllInspectionDTO>
        {
            Items = inspectionDtos,
            TotalCount = pagedInspections.TotalCount,
            PageNumber = pagedInspections.PageNumber,
            PageSize = pagedInspections.PageSize
        };

        _logger.LogInformation($"Retrieved {result.Items.Count} inspections from page {result.PageNumber}");
        return result;
    }
}