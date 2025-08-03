using Application.Contracts.AuthService;
using Application.Contracts.JwtService;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Auth.Command;

using AutoMapper;

using FluentValidation;

using MediatR;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthUserDTO>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IAppLogger<LoginCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IValidator<LoginCommand> _validator;

    public LoginCommandHandler(
        IJwtService jwtService,
        IAppLogger<LoginCommandHandler> logger,
        IMapper mapper,
        IValidator<LoginCommand> validator,
        IUserRepository userRepository)
    {
        _jwtService = jwtService;
        _logger = logger;
        _mapper = mapper;
        _validator = validator;
        _userRepository = userRepository;
    }

    public async Task<AuthUserDTO> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"LoginCommand - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        await ValidateBusinessRulesAsync(request);

        // Find user and generate token
        var user = await _userRepository.GetByEmailAsync(request.Email);
        var roles = await _userRepository.GetRolesAsync(user!);
        var token = _jwtService.GenerateToken(user!.Id, user.Email!, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Map to DTO
        var userDto = _mapper.Map<AuthUserDTO>(user);
        userDto.Roles = roles;
        userDto.Token = token;
        userDto.RefreshToken = refreshToken;
        userDto.Expires = DateTime.UtcNow.AddHours(1);

        _logger.LogInformation("Login successful for user: {UserId}", user.Id);
        return userDto;
    }

    /// <summary>
    /// Validates authentication business rules
    /// </summary>
    /// <param name="request">The login request</param>
    /// <exception cref="EntityNotFoundException">User not found</exception>
    /// <exception cref="UnauthorizedAccessException">Invalid credentials or inactive account</exception>
    private async Task ValidateBusinessRulesAsync(LoginCommand request)
    {
        // Check user exists
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            var errorMessage = $"User not found for email: {request.Email}";
            _logger.LogWarning(errorMessage);
            // Don't reveal if email exists - return generic message
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Check account is active
        if (!user.IsActive)
        {
            var errorMessage = $"Inactive account for user: {user.Id}";
            _logger.LogWarning(errorMessage);
            throw new UnauthorizedAccessException("Account is inactive");
        }

        // Check password
        var isPasswordValid = await _userRepository.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            var errorMessage = $"Invalid password for user: {user.Id}";
            _logger.LogWarning(errorMessage);
            throw new UnauthorizedAccessException("Invalid email or password");
        }
    }
}