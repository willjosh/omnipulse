using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Models.PaginationModels;

using AutoMapper;

using FluentValidation;

using MediatR;

namespace Application.Features.ServiceReminders.Query.GetAllServiceReminders;

public class GetAllServiceRemindersQueryHandler : IRequestHandler<GetAllServiceRemindersQuery, PagedResult<ServiceReminderDTO>>
{
    private readonly IServiceReminderRepository _serviceReminderRepository;
    private readonly IValidator<GetAllServiceRemindersQuery> _validator;
    private readonly IAppLogger<GetAllServiceRemindersQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetAllServiceRemindersQueryHandler(
        IServiceReminderRepository serviceReminderRepository,
        IValidator<GetAllServiceRemindersQuery> validator,
        IAppLogger<GetAllServiceRemindersQueryHandler> logger,
        IMapper mapper)
    {
        _serviceReminderRepository = serviceReminderRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<PagedResult<ServiceReminderDTO>> Handle(GetAllServiceRemindersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)}() - Handling {nameof(GetAllServiceRemindersQuery)}({nameof(GetAllServiceRemindersQuery.Parameters)}: {request.Parameters})");

        // Validate request
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(GetAllServiceRemindersQueryValidator)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Get all service reminders from the repository
        var result = await _serviceReminderRepository.GetAllServiceRemindersPagedAsync(request.Parameters);

        // Map the service reminders to DTOs using AutoMapper
        var serviceReminderDTOs = _mapper.Map<List<ServiceReminderDTO>>(result.Items);

        var pagedResult = new PagedResult<ServiceReminderDTO>
        {
            Items = serviceReminderDTOs,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        _logger.LogInformation($"Returning {pagedResult.TotalCount} service reminders for page {pagedResult.PageNumber} with page size {pagedResult.PageSize}");
        return pagedResult;
    }
}