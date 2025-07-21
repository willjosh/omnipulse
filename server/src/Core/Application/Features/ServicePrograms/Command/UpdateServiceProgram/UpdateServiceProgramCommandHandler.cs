using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.ServicePrograms.Command.UpdateServiceProgram;

public class UpdateServiceProgramCommandHandler : IRequestHandler<UpdateServiceProgramCommand, int>
{
    private readonly IServiceProgramRepository _serviceProgramRepository;
    private readonly IValidator<UpdateServiceProgramCommand> _validator;
    private readonly IAppLogger<UpdateServiceProgramCommandHandler> _logger;
    private readonly IMapper _mapper;

    public UpdateServiceProgramCommandHandler(
        IServiceProgramRepository serviceProgramRepository,
        IValidator<UpdateServiceProgramCommand> validator,
        IAppLogger<UpdateServiceProgramCommandHandler> logger,
        IMapper mapper)
    {
        _serviceProgramRepository = serviceProgramRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> Handle(UpdateServiceProgramCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(UpdateServiceProgramCommand)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Check if service program exists
        var existingProgram = await _serviceProgramRepository.GetByIdAsync(request.ServiceProgramID);
        if (existingProgram == null)
        {
            var errorMessage = $"{nameof(ServiceProgram)} {nameof(ServiceProgram.ID)} not found: {request.ServiceProgramID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(nameof(ServiceProgram), nameof(ServiceProgram.ID), request.ServiceProgramID.ToString());
        }

        // Check for duplicate name if changed
        if (!string.Equals(existingProgram.Name, request.Name, StringComparison.Ordinal) &&
            !await _serviceProgramRepository.IsNameUniqueAsync(request.Name))
        {
            var errorMessage = $"{nameof(ServiceProgram)} {nameof(ServiceProgram.Name)} already exists: {request.Name}";
            _logger.LogError(errorMessage);
            throw new DuplicateEntityException(nameof(ServiceProgram), nameof(ServiceProgram.Name), request.Name);
        }

        // Map request to entity (update properties)
        _mapper.Map(request, existingProgram);

        // Update & Save
        _serviceProgramRepository.Update(existingProgram);
        await _serviceProgramRepository.SaveChangesAsync();

        _logger.LogInformation($"Successfully updated {nameof(ServiceProgram)} with {nameof(ServiceProgram.ID)}: {request.ServiceProgramID}");

        // Return ID
        return existingProgram.ID;
    }
}