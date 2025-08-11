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
    private readonly ISender _sender;
    private readonly IValidator<DeleteServiceProgramCommand> _validator;
    private readonly IAppLogger<DeleteServiceProgramCommandHandler> _logger;

    public DeleteServiceProgramCommandHandler(
        IServiceProgramRepository serviceProgramRepository,
        ISender sender,
        IValidator<DeleteServiceProgramCommand> validator,
        IAppLogger<DeleteServiceProgramCommandHandler> logger)
    {
        _serviceProgramRepository = serviceProgramRepository;
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

        // Cascade delete behaviour is configured in ServiceScheduleConfiguration and XrefServiceProgramVehicleConfiguration
        _serviceProgramRepository.Delete(targetServiceProgram);

        // Save changes
        await _serviceProgramRepository.SaveChangesAsync();

        // Trigger sync to handle reminders post-delete
        await _sender.Send(new SyncServiceRemindersCommand(), cancellationToken);

        // Return the deleted ServiceProgramID
        return request.ServiceProgramID;
    }
}