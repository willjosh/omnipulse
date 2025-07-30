using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Models.PaginationModels;

using AutoMapper;

using FluentValidation;

using MediatR;

namespace Application.Features.ServiceTasks.Query.GetAllServiceTask;

public class GetAllServiceTaskQueryHandler : IRequestHandler<GetAllServiceTaskQuery, PagedResult<ServiceTaskDTO>>
{
    private readonly IServiceTaskRepository _serviceTaskRepository;
    private readonly IValidator<GetAllServiceTaskQuery> _validator;
    private readonly IAppLogger<GetAllServiceTaskQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetAllServiceTaskQueryHandler(
        IServiceTaskRepository serviceTaskRepository,
        IValidator<GetAllServiceTaskQuery> validator,
        IAppLogger<GetAllServiceTaskQueryHandler> logger,
        IMapper mapper)
    {
        _serviceTaskRepository = serviceTaskRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<PagedResult<ServiceTaskDTO>> Handle(GetAllServiceTaskQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllServiceTaskQuery");
        // validate the request
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"GetAllServiceTask - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // get all service tasks from the repository
        var result = await _serviceTaskRepository.GetAllServiceTasksPagedAsync(request.Parameters);

        // map the service tasks to DTOs
        var serviceTaskDTOs = _mapper.Map<List<ServiceTaskDTO>>(result.Items);

        var pagedResult = new PagedResult<ServiceTaskDTO>
        {
            Items = serviceTaskDTOs,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        _logger.LogInformation($"Returning {pagedResult.TotalCount} service tasks for page {pagedResult.PageNumber} with page size {pagedResult.PageSize}");
        return pagedResult;
    }
}