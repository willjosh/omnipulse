using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using FluentValidation;

using MediatR;

namespace Application.Features.Users.Command.UpdateTechnician;

public class UpdateTechnicianCommandHandler : IRequestHandler<UpdateTechnicianCommand, string>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<UpdateTechnicianCommandHandler> _logger;
    private readonly IValidator<UpdateTechnicianCommand> _validator;

    public UpdateTechnicianCommandHandler(
        IUserRepository userRepository,
        IMapper mapper,
        IAppLogger<UpdateTechnicianCommandHandler> logger,
        IValidator<UpdateTechnicianCommand> validator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<string> Handle(UpdateTechnicianCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"UpdateTechnician - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        var user = await _userRepository.GetByIdAsync(request.Id);
        if (user == null)
        {
            _logger.LogWarning($"UpdateTechnician - User with ID {request.Id} not found");
            throw new EntityNotFoundException("User", "Id", request.Id);
        }

        // check if update is needed
        if (!request.ShouldUpdate)
        {
            _logger.LogInformation($"UpdateTechnician - No updates needed for user with ID: {user.Id}");
            return user.Id;
        }

        // update user properties
        if (request.ShouldUpdateFirstName)
        {
            user.FirstName = request.FirstName!;
        }

        if (request.ShouldUpdateLastName)
        {
            user.LastName = request.LastName!;
        }

        if (request.ShouldUpdateHireDate)
        {
            user.HireDate = request.HireDate!.Value;
        }

        if (request.ShouldUpdateIsActive)
        {
            user.IsActive = request.IsActive!.Value;
        }

        // update user
        user.UpdatedAt = DateTime.UtcNow;
        var result = await _userRepository.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogError($"UpdateTechnician - User update failed: {errors}");
            throw new InvalidOperationException($"Failed to update user: {errors}");
        }
        _logger.LogInformation($"UpdateTechnician - Successfully updated user with ID: {user.Id}");

        return user.Id;
    }
}