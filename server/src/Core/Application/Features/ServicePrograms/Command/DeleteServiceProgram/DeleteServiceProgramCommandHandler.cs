using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServiceReminders.Command.SyncServiceReminders;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.ServicePrograms.Command.DeleteServiceProgram;

public class DeleteServiceProgramCommandHandler : IRequestHandler<DeleteServiceProgramCommand, int>
{
    private readonly IServiceProgramRepository _serviceProgramRepository;
    private readonly IServiceScheduleRepository _serviceScheduleRepository;
    private readonly IServiceReminderRepository _serviceReminderRepository;
    private readonly ISender _sender;
    private readonly IValidator<DeleteServiceProgramCommand> _validator;
    private readonly IAppLogger<DeleteServiceProgramCommandHandler> _logger;

    public DeleteServiceProgramCommandHandler(
        IServiceProgramRepository serviceProgramRepository,
        IServiceScheduleRepository serviceScheduleRepository,
        IServiceReminderRepository serviceReminderRepository,
        ISender sender,
        IValidator<DeleteServiceProgramCommand> validator,
        IAppLogger<DeleteServiceProgramCommandHandler> logger)
    {
        _serviceProgramRepository = serviceProgramRepository;
        _serviceScheduleRepository = serviceScheduleRepository;
        _serviceReminderRepository = serviceReminderRepository;
        _sender = sender;
        _validator = validator;
        _logger = logger;
    }

    public async Task<int> Handle(DeleteServiceProgramCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(DeleteServiceProgramCommand)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Get ServiceProgram by ID
        var targetServiceProgram = await _serviceProgramRepository.GetByIdAsync(request.ServiceProgramID);
        if (targetServiceProgram == null)
        {
            _logger.LogError($"{nameof(ServiceProgram)} with {nameof(ServiceProgram.ID)} {request.ServiceProgramID} not found.");
            throw new EntityNotFoundException(nameof(ServiceProgram), nameof(ServiceProgram.ID), request.ServiceProgramID.ToString());
        }

        // Soft-delete the ServiceProgram instead of hard-deleting to avoid FK conflicts and preserve history
        // Also soft-delete all associated ServiceSchedules and remove non-final reminders
        targetServiceProgram.IsActive = false;

        var schedules = await _serviceScheduleRepository.GetAllByServiceProgramIDAsync(targetServiceProgram.ID);

        foreach (var schedule in schedules)
        {
            // Soft-delete schedule so it's excluded from future queries/generation
            schedule.IsSoftDeleted = true;
            _serviceScheduleRepository.Update(schedule);

            // Clean up non-final, non-workorder-linked reminders to prevent clutter
            await _serviceReminderRepository.DeleteNonFinalRemindersForScheduleAsync(schedule.ID, cancellationToken);
        }

        _serviceProgramRepository.Update(targetServiceProgram);
        await _serviceProgramRepository.SaveChangesAsync();

        // Trigger sync to handle reminders post-delete
        await _sender.Send(new SyncServiceRemindersCommand(), cancellationToken);

        // Return the deleted ServiceProgramID
        return request.ServiceProgramID;
    }
}