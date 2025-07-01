using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.Users.Command.CreateTechnician;

public class CreateTechnicianCommandHandler : IRequestHandler<CreateTechnicianCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<CreateTechnicianCommandHandler> _logger;
    private readonly IValidator<CreateTechnicianCommand> _validator;

    public CreateTechnicianCommandHandler(
        IUserRepository userRepository,
        IMapper mapper,
        IAppLogger<CreateTechnicianCommandHandler> logger,
        IValidator<CreateTechnicianCommand> validator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Guid> Handle(CreateTechnicianCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"CreateTechnician - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Map request to User
        var user = _mapper.Map<User>(request);
        user.UserName = request.Email;
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        // Validate business rules
        await ValidateBusinessRuleAsync(user);

        // Add new technician
        var result = await _userRepository.AddAsync(user, request.Password);

        // Check if creation succeeded
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogError($"CreateTechnician - User creation failed: {errors}");
            throw new InvalidOperationException($"Failed to create user: {errors}");
        }

        _logger.LogInformation($"CreateTechnician - Successfully created user with email: {user.Email}");
        return Guid.Parse(user.Id);
    }

    private async Task ValidateBusinessRuleAsync(User user)
    {
        // Check for duplicate email in the db
        if (await _userRepository.EmailExistsAsync(user.Email!))
        {
            var errorMessage = $"Email already exists: {user.Email}";
            _logger.LogError($"CreateTechnician - {errorMessage}");
            throw new DuplicateEntityException(typeof(User).ToString(), "Email", user.Email!);
        }
    }
}