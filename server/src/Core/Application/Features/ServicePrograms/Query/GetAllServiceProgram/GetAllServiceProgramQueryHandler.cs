using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Models.PaginationModels;

using AutoMapper;

using FluentValidation;

using MediatR;

namespace Application.Features.ServicePrograms.Query.GetAllServiceProgram;

public class GetAllServiceProgramQueryHandler : IRequestHandler<GetAllServiceProgramQuery, PagedResult<GetAllServiceProgramDTO>>
{
    private readonly IServiceProgramRepository _serviceProgramRepository;
    private readonly IValidator<GetAllServiceProgramQuery> _validator;
    private readonly IAppLogger<GetAllServiceProgramQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetAllServiceProgramQueryHandler(
        IServiceProgramRepository serviceProgramRepository,
        IValidator<GetAllServiceProgramQuery> validator,
        IAppLogger<GetAllServiceProgramQueryHandler> logger,
        IMapper mapper)
    {
        _serviceProgramRepository = serviceProgramRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<PagedResult<GetAllServiceProgramDTO>> Handle(GetAllServiceProgramQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)}() - Handling {nameof(GetAllServiceProgramQuery)}");

        // Validate request
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(GetAllServiceProgramQueryValidator)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Get all service programs from the repository
        var result = await _serviceProgramRepository.GetAllServiceProgramsPagedAsync(request.Parameters);

        // Map the service programs to DTOs
        var serviceProgramDTOs = _mapper.Map<List<GetAllServiceProgramDTO>>(result.Items);

        var pagedResult = new PagedResult<GetAllServiceProgramDTO>
        {
            Items = serviceProgramDTOs,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        _logger.LogInformation($"Returning {pagedResult.TotalCount} service programs for page {pagedResult.PageNumber} with page size {pagedResult.PageSize}");
        return pagedResult;
    }
}