using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServiceReminders.Command.SyncServiceReminders;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.ServiceSchedules.Command.UpdateServiceSchedule;

public sealed class UpdateServiceScheduleCommandHandler : IRequestHandler<UpdateServiceScheduleCommand, int>
{
    private readonly IServiceScheduleRepository _serviceScheduleRepository;
    private readonly IServiceProgramRepository _serviceProgramRepository;
    private readonly IXrefServiceScheduleServiceTaskRepository _xrefServiceScheduleServiceTaskRepository;
    private readonly IServiceReminderRepository _serviceReminderRepository;
    private readonly ISender _sender;
    private readonly IValidator<UpdateServiceScheduleCommand> _validator;
    private readonly IAppLogger<UpdateServiceScheduleCommandHandler> _logger;
    private readonly IMapper _mapper;

    public UpdateServiceScheduleCommandHandler(
        IServiceScheduleRepository serviceScheduleRepository,
        IServiceProgramRepository serviceProgramRepository,
        IXrefServiceScheduleServiceTaskRepository xrefServiceScheduleServiceTaskRepository,
        IServiceReminderRepository serviceReminderRepository,
        ISender sender,
        IValidator<UpdateServiceScheduleCommand> validator,
        IAppLogger<UpdateServiceScheduleCommandHandler> logger,
        IMapper mapper)
    {
        _serviceScheduleRepository = serviceScheduleRepository;
        _serviceProgramRepository = serviceProgramRepository;
        _xrefServiceScheduleServiceTaskRepository = xrefServiceScheduleServiceTaskRepository;
        _serviceReminderRepository = serviceReminderRepository;
        _sender = sender;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> Handle(UpdateServiceScheduleCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"UpdateServiceScheduleCommand - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Check if service schedule exists
        var existingSchedule = await _serviceScheduleRepository.GetByIdAsync(request.ServiceScheduleID);
        if (existingSchedule == null)
        {
            var errorMessage = $"ServiceSchedule ID not found: {request.ServiceScheduleID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(typeof(ServiceSchedule).ToString(), "ServiceScheduleID", request.ServiceScheduleID.ToString());
        }

        // Validate ServiceProgramID exists
        if (!await _serviceProgramRepository.ExistsAsync(request.ServiceProgramID))
        {
            var errorMessage = $"ServiceProgram ID not found: {request.ServiceProgramID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(typeof(ServiceProgram).ToString(), "ServiceProgramID", request.ServiceProgramID.ToString());
        }

        // Map request to existing entity (updates properties)
        _mapper.Map(request, existingSchedule);

        // Replace all xrefs for this schedule
        await _xrefServiceScheduleServiceTaskRepository.RemoveAllForScheduleAsync(existingSchedule.ID);
        var newXrefs = request.ServiceTaskIDs
            .Select(serviceTaskID => new XrefServiceScheduleServiceTask
            {
                ServiceScheduleID = existingSchedule.ID,
                ServiceTaskID = serviceTaskID,
                ServiceSchedule = null!, // Navigation Property
                ServiceTask = null!, // Navigation Property
            })
            .ToList();
        await _xrefServiceScheduleServiceTaskRepository.AddRangeAsync(newXrefs);

        // Persist schedule changes
        _serviceScheduleRepository.Update(existingSchedule);
        await _serviceScheduleRepository.SaveChangesAsync();

        _logger.LogInformation($"Service schedule '{existingSchedule.Name}' updated successfully with ID: {existingSchedule.ID}");

        // Business Rules after update:
        // - Affect only future reminders: cancel UPCOMING/DUE_SOON without WorkOrder
        // Cancel future reminders for this schedule
        await _serviceReminderRepository.DeleteNonFinalRemindersForScheduleAsync(request.ServiceScheduleID, cancellationToken);

        // Trigger regeneration to create new reminders based on updated schedule
        var sendTask = _sender.Send(new SyncServiceRemindersCommand(), cancellationToken);
        if (sendTask != null)
        {
            var syncResult = await sendTask;
            if (syncResult == null || !syncResult.Success)
            {
                var error = syncResult?.ErrorMessage ?? "Unknown error";
                _logger.LogWarning($"{nameof(UpdateServiceScheduleCommandHandler)} - Failed to sync reminders after update: {error}");
            }
        }
        else
        {
            _logger.LogWarning($"{nameof(UpdateServiceScheduleCommandHandler)} - ISender.Send returned null task; skipping reminder regeneration.");
        }

        return existingSchedule.ID;
    }
}