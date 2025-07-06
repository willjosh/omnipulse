using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.ServiceTasks.Command.CreateServiceTask;

/// <summary>
/// Handles the <see cref="CreateServiceTaskCommand"/>
/// </summary>
public sealed class CreateServiceTaskCommandHandler : IRequestHandler<CreateServiceTaskCommand, int>
{
    private readonly IServiceTaskRepository _serviceTaskRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<CreateServiceTaskCommandHandler> _logger;
    private readonly IValidator<CreateServiceTaskCommand> _validator;

    public CreateServiceTaskCommandHandler(
        IServiceTaskRepository serviceTaskRepository,
        IValidator<CreateServiceTaskCommand> validator,
        IAppLogger<CreateServiceTaskCommandHandler> logger,
        IMapper mapper)
    {
        _serviceTaskRepository = serviceTaskRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    /// <param name="request">The <see cref="CreateServiceTaskCommand"/> containing service task details.</param>
    /// <param name="cancellationToken">Token to observe while awaiting async operations.</param>
    /// <returns>The ID of the newly created <see cref="ServiceTask"/>.</returns>
    /// <exception cref="BadRequestException">Thrown when validation fails.</exception>
    /// <exception cref="DuplicateEntityException">Thrown when a service task with the same name already exists.</exception>
    public async Task<int> Handle(CreateServiceTaskCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"CreateServiceTaskCommand - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Map request to service task domain entity
        var serviceTask = _mapper.Map<ServiceTask>(request);

        // Validate business rules
        await ValidateBusinessRuleAsync(serviceTask);

        // Add new service task
        var newServiceTask = await _serviceTaskRepository.AddAsync(serviceTask);

        // Save changes
        await _serviceTaskRepository.SaveChangesAsync();

        _logger.LogInformation($"Service task '{newServiceTask.Name}' created successfully with ID: {newServiceTask.ID}");

        // Return service task ID
        return newServiceTask.ID;
    }

    /// <param name="serviceTask">The <see cref="ServiceTask"/> entity to validate.</param>
    /// <exception cref="DuplicateEntityException">Thrown when a service task with the same name already exists.</exception>
    private async Task ValidateBusinessRuleAsync(ServiceTask serviceTask)
    {
        // Check for duplicate name in the database
        if (await _serviceTaskRepository.DoesNameExistAsync(serviceTask.Name))
        {
            var errorMessage = $"Service task name already exists: {serviceTask.Name}";
            _logger.LogError(errorMessage);
            throw new DuplicateEntityException(typeof(ServiceTask).ToString(), "Name", serviceTask.Name);
        }
    }
}