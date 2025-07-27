using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.ServiceSchedules.Command.CreateServiceSchedule;

public sealed class CreateServiceScheduleCommandHandler : IRequestHandler<CreateServiceScheduleCommand, int>
{
    private readonly IServiceScheduleRepository _serviceScheduleRepository;
    private readonly IServiceProgramRepository _serviceProgramRepository;
    private readonly IXrefServiceScheduleServiceTaskRepository _xrefServiceScheduleServiceTaskRepository;
    private readonly IValidator<CreateServiceScheduleCommand> _validator;
    private readonly IAppLogger<CreateServiceScheduleCommandHandler> _logger;
    private readonly IMapper _mapper;

    public CreateServiceScheduleCommandHandler(
        IServiceScheduleRepository serviceScheduleRepository,
        IServiceProgramRepository serviceProgramRepository,
        IXrefServiceScheduleServiceTaskRepository xrefServiceScheduleServiceTaskRepository,
        IValidator<CreateServiceScheduleCommand> validator,
        IAppLogger<CreateServiceScheduleCommandHandler> logger,
        IMapper mapper)
    {
        _serviceScheduleRepository = serviceScheduleRepository;
        _serviceProgramRepository = serviceProgramRepository;
        _xrefServiceScheduleServiceTaskRepository = xrefServiceScheduleServiceTaskRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateServiceScheduleCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"CreateServiceScheduleCommand - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Validate ServiceProgramID exists
        if (!await _serviceProgramRepository.ExistsAsync(request.ServiceProgramID))
        {
            var errorMessage = $"ServiceProgram ID not found: {request.ServiceProgramID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(typeof(ServiceProgram).ToString(), "ServiceProgramID", request.ServiceProgramID.ToString());
        }

        // Map request to domain entity
        var serviceSchedule = _mapper.Map<ServiceSchedule>(request);

        // Add new service schedule
        var newServiceSchedule = await _serviceScheduleRepository.AddAsync(serviceSchedule);

        // Add XrefServiceScheduleServiceTask for each ServiceTaskID
        var xrefServiceScheduleServiceTasks = request.ServiceTaskIDs
            .Select(serviceTaskID => new XrefServiceScheduleServiceTask
            {
                ServiceScheduleID = newServiceSchedule.ID,
                ServiceTaskID = serviceTaskID,
                ServiceSchedule = null!, // Navigation Property
                ServiceTask = null!, // Navigation Property
            })
            .ToList();

        await _xrefServiceScheduleServiceTaskRepository.AddRangeAsync(xrefServiceScheduleServiceTasks);
        await _serviceScheduleRepository.SaveChangesAsync();

        _logger.LogInformation($"Service schedule '{newServiceSchedule.Name}' created successfully with ID: {newServiceSchedule.ID}");

        return newServiceSchedule.ID;
    }
}