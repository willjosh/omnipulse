using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.ServiceTasks.Command.UpdateServiceTask;

/// <summary>
/// Handles <see cref="UpdateServiceTaskCommand"/>
/// </summary>
public class UpdateServiceTaskCommandHandler : IRequestHandler<UpdateServiceTaskCommand, int>
{
    private readonly IServiceTaskRepository _serviceTaskRepository;
    private readonly IValidator<UpdateServiceTaskCommand> _validator;
    private readonly IAppLogger<UpdateServiceTaskCommandHandler> _logger;
    private readonly IMapper _mapper;

    public UpdateServiceTaskCommandHandler(
        IServiceTaskRepository serviceTaskRepository,
        IValidator<UpdateServiceTaskCommand> validator,
        IAppLogger<UpdateServiceTaskCommandHandler> logger,
        IMapper mapper)
    {
        _serviceTaskRepository = serviceTaskRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    /// <param name="request">The <see cref="UpdateServiceTaskCommand"/> containing updated service task details.</param>
    /// <param name="cancellationToken">Token to observe while awaiting async operations.</param>
    /// <returns>The ID of the updated <see cref="ServiceTask"/>.</returns>
    /// <exception cref="BadRequestException">Thrown when validation fails.</exception>
    /// <exception cref="EntityNotFoundException">Thrown when the service task with the specified ID does not exist.</exception>
    /// <exception cref="DuplicateEntityException">Thrown when a service task with the same name already exists (excluding the current service task).</exception>
    public async Task<int> Handle(UpdateServiceTaskCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"UpdateServiceTaskCommand - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Check if service task exists
        var existingServiceTask = await _serviceTaskRepository.GetByIdAsync(request.ServiceTaskID);
        if (existingServiceTask == null)
        {
            var errorMessage = $"Service Task ID not found: {request.ServiceTaskID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(typeof(ServiceTask).ToString(), "ServiceTaskID", request.ServiceTaskID.ToString());
        }

        // Validate business rules
        await ValidateBusinessRulesAsync(request);

        // Map request to service task entity (this will update the existing service task properties)
        _mapper.Map(request, existingServiceTask);

        // Update service task
        _serviceTaskRepository.Update(existingServiceTask);

        // Save changes
        await _serviceTaskRepository.SaveChangesAsync();

        _logger.LogInformation($"Successfully updated service task with ID: {request.ServiceTaskID}");

        // Return service task ID
        return existingServiceTask.ID;
    }

    /// <param name="request">The <see cref="UpdateServiceTaskCommand"/> to validate.</param>
    /// <exception cref="DuplicateEntityException">Thrown when name duplicates are detected (excluding the current service task).</exception>
    private async Task ValidateBusinessRulesAsync(UpdateServiceTaskCommand request)
    {
        // Check for duplicate name in the database (excluding current service task)
        if (await _serviceTaskRepository.DoesNameExistExcludingIdAsync(request.Name, request.ServiceTaskID))
        {
            var errorMessage = $"Service task name already exists: {request.Name}";
            _logger.LogError(errorMessage);
            throw new DuplicateEntityException(typeof(ServiceTask).ToString(), "Name", request.Name);
        }
    }
}