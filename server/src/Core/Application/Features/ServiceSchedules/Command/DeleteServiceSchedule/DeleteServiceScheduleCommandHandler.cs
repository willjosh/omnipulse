using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServiceReminders.Command.GenerateServiceReminders;

using Domain.Entities;

using MediatR;

namespace Application.Features.ServiceSchedules.Command.DeleteServiceSchedule;

public class DeleteServiceScheduleCommandHandler : IRequestHandler<DeleteServiceScheduleCommand, int>
{
    private readonly IServiceScheduleRepository _serviceScheduleRepository;
    private readonly IXrefServiceScheduleServiceTaskRepository _xrefServiceScheduleServiceTaskRepository;
    private readonly IServiceReminderRepository _serviceReminderRepository;
    private readonly ISender _sender;
    private readonly IAppLogger<DeleteServiceScheduleCommandHandler> _logger;

    public DeleteServiceScheduleCommandHandler(
        IServiceScheduleRepository serviceScheduleRepository,
        IXrefServiceScheduleServiceTaskRepository xrefServiceScheduleServiceTaskRepository,
        IServiceReminderRepository serviceReminderRepository,
        ISender sender,
        IAppLogger<DeleteServiceScheduleCommandHandler> logger)
    {
        _serviceScheduleRepository = serviceScheduleRepository;
        _xrefServiceScheduleServiceTaskRepository = xrefServiceScheduleServiceTaskRepository;
        _serviceReminderRepository = serviceReminderRepository;
        _sender = sender;
        _logger = logger;
    }

    public async Task<int> Handle(DeleteServiceScheduleCommand request, CancellationToken cancellationToken)
    {
        // Validate ServiceScheduleID
        _logger.LogInformation($"Validating ServiceSchedule with ID: {request.ServiceScheduleID}");
        var serviceSchedule = await _serviceScheduleRepository.GetByIdAsync(request.ServiceScheduleID);
        if (serviceSchedule == null)
        {
            _logger.LogError($"ServiceSchedule with ID {request.ServiceScheduleID} not found.");
            throw new EntityNotFoundException(typeof(ServiceSchedule).ToString(), "ID", request.ServiceScheduleID.ToString());
        }

        // Remove all xrefs for this schedule before soft-deleting
        _logger.LogInformation($"Removing all service task links for ServiceSchedule ID: {request.ServiceScheduleID}");
        await _xrefServiceScheduleServiceTaskRepository.RemoveAllForScheduleAsync(request.ServiceScheduleID);

        // Soft-delete schedule
        serviceSchedule.IsSoftDeleted = true;
        _serviceScheduleRepository.Update(serviceSchedule);
        await _serviceScheduleRepository.SaveChangesAsync();
        _logger.LogInformation($"ServiceSchedule with ID: {request.ServiceScheduleID} soft-deleted");

        // Cancel future reminders (UPCOMING/DUE_SOON without WorkOrder)
        var cancelled = await _serviceReminderRepository.CancelFutureRemindersForScheduleAsync(request.ServiceScheduleID);
        _logger.LogInformation("Cancelled {Count} future reminders for soft-deleted schedule {ScheduleID}", cancelled, request.ServiceScheduleID);

        // Trigger regeneration to refresh state for other schedules
        await _sender.Send(new GenerateServiceRemindersCommand(), cancellationToken);

        return request.ServiceScheduleID;
    }
}