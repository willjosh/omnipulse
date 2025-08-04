using System;

using Application.Contracts.AuthService;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.Auth.Command.Register;

public class RegisterOperatorCommandHandler : IRequestHandler<RegisterOperatorCommand, Guid>
{

    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<RegisterOperatorCommandHandler> _logger;
    private readonly IValidator<RegisterOperatorCommand> _validator;

    public RegisterOperatorCommandHandler(
        IUserRepository userRepository,
        IMapper mapper,
        IAppLogger<RegisterOperatorCommandHandler> logger,
        IValidator<RegisterOperatorCommand> validator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Guid> Handle(RegisterOperatorCommand request, CancellationToken cancellationToken)
    {
        // validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"Create Operator - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // map request to User
        var user = _mapper.Map<User>(request);
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        // validate business rules
        await ValidateBusinessRuleAsync(user);

        // Add new technician
        var result = await _userRepository.AddAsyncWithRole(user, request.Password, UserRole.FleetManager);

        // Check if creation succeeded
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogError($"Create Operator - User creation failed: {errors}");
            throw new InvalidOperationException($"Failed to create user: {errors}");
        }

        _logger.LogInformation($"Create Operator - User created successfully with ID: {user.Id}");
        return Guid.Parse(user.Id);
    }

    private async Task ValidateBusinessRuleAsync(User user)
    {
        // Check for duplicate email in the db
        if (await _userRepository.EmailExistsAsync(user.Email!))
        {
            var errorMessage = $"Email already exists: {user.Email}";
            _logger.LogError($"Create Operator - {errorMessage}");
            throw new DuplicateEntityException(typeof(User).ToString(), "Email", user.Email!);
        }
    }
}