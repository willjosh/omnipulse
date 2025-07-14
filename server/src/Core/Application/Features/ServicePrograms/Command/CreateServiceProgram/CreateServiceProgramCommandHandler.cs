using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.ServicePrograms.Command.CreateServiceProgram;

public sealed class CreateServiceProgramCommandHandler : IRequestHandler<CreateServiceProgramCommand, int>
{
    private readonly IServiceProgramRepository _serviceProgramRepository;
    private readonly IValidator<CreateServiceProgramCommand> _validator;
    private readonly IAppLogger<CreateServiceProgramCommandHandler> _logger;
    private readonly IMapper _mapper;

    public CreateServiceProgramCommandHandler(
        IServiceProgramRepository serviceProgramRepository,
        IValidator<CreateServiceProgramCommand> validator,
        IAppLogger<CreateServiceProgramCommandHandler> logger,
        IMapper mapper)
    {
        _serviceProgramRepository = serviceProgramRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateServiceProgramCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(CreateServiceProgramCommand)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Check for duplicate name (case-sensitive)
        if (!await _serviceProgramRepository.IsNameUniqueAsync(request.Name))
        {
            _logger.LogError($"{nameof(ServiceProgram)} '{nameof(ServiceProgram.Name)}' already exists: {request.Name}");
            throw new DuplicateEntityException(nameof(ServiceProgram), nameof(ServiceProgram.Name), request.Name);
        }

        // Map request to domain entity
        var serviceProgram = _mapper.Map<ServiceProgram>(request);

        // Add new service program
        var newServiceProgram = await _serviceProgramRepository.AddAsync(serviceProgram);
        await _serviceProgramRepository.SaveChangesAsync();

        _logger.LogInformation($"{nameof(ServiceProgram)} '{newServiceProgram.Name}' created successfully with {nameof(ServiceProgram.ID)}: {newServiceProgram.ID}");

        return newServiceProgram.ID;
    }
}