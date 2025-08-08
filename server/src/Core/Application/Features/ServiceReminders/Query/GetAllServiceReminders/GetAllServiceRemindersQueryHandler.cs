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

        // Set calculated properties for each DTO
        foreach (var dto in serviceReminderDTOs)
        {
            var serviceReminder = result.Items.FirstOrDefault(x => x.ID == dto.ID);
            if (serviceReminder != null)
            {
                // Calculate occurrence number based on position among reminders for the same vehicle and service schedule
                dto.OccurrenceNumber = CalculateOccurrenceNumber(serviceReminder, result.Items);
            }
        }

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

    /// <summary>
    /// Calculate the occurrence number for a reminder based on its position among reminders
    /// for the same vehicle and service schedule, ordered by due date/mileage
    /// </summary>
    private static int CalculateOccurrenceNumber(Domain.Entities.ServiceReminder currentReminder, IReadOnlyList<Domain.Entities.ServiceReminder> allReminders)
    {
        // Get all reminders for the same vehicle and service schedule
        var sameScheduleReminders = allReminders
            .Where(r => r.VehicleID == currentReminder.VehicleID &&
                       r.ServiceScheduleID == currentReminder.ServiceScheduleID)
            .OrderBy(r => r.DueDate ?? DateTime.MaxValue)
            .ThenBy(r => r.DueMileage ?? double.MaxValue)
            .ThenBy(r => r.ID) // Use ID as tiebreaker for consistent ordering
            .ToList();

        // Find the position of the current reminder (1-based index)
        var occurrenceNumber = sameScheduleReminders.FindIndex(r => r.ID == currentReminder.ID) + 1;

        // Return at least 1 if not found (shouldn't happen in normal cases tho)
        return occurrenceNumber > 0 ? occurrenceNumber : 1;
    }
}