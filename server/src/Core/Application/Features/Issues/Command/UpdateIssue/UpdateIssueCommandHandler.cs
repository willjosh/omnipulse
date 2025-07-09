using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.Issues.Command.UpdateIssue;

public class UpdateIssueCommandHandler : IRequestHandler<UpdateIssueCommand, int>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<UpdateIssueCommandHandler> _logger;
    private readonly IValidator<UpdateIssueCommand> _validator;

    public UpdateIssueCommandHandler(
        IIssueRepository issueRepository,
        IVehicleRepository vehicleRepository,
        IUserRepository userRepository,
        IValidator<UpdateIssueCommand> validator,
        IAppLogger<UpdateIssueCommandHandler> logger,
        IMapper mapper)
    {
        _issueRepository = issueRepository;
        _vehicleRepository = vehicleRepository;
        _userRepository = userRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> Handle(UpdateIssueCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"UpdateIssueCommand - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Check if issue exists
        var existingIssue = await _issueRepository.GetByIdAsync(request.IssueID);
        if (existingIssue == null)
        {
            var errorMessage = $"Issue ID not found: {request.IssueID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(typeof(Issue).ToString(), "IssueID", request.IssueID.ToString());
        }

        // Validate business rules
        await ValidateBusinessRulesAsync(request);

        // Map request to issue domain entity
        _mapper.Map(request, existingIssue);

        // Update issue
        _issueRepository.Update(existingIssue);
        await _issueRepository.SaveChangesAsync();

        _logger.LogInformation($"Successfully updated issue with ID: {existingIssue.ID}");
        return existingIssue.ID;
    }

    private async Task ValidateBusinessRulesAsync(UpdateIssueCommand request)
    {
        // Check if vehicle exists
        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleID);
        if (vehicle == null)
        {
            var errorMessage = $"Vehicle ID not found: {request.VehicleID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(typeof(Vehicle).ToString(), "VehicleID", request.VehicleID.ToString());
        }

        // Check if user exists
        var user = await _userRepository.GetByIdAsync(request.ReportedByUserID);
        if (user == null)
        {
            var errorMessage = $"ReportedByUserID not found: {request.ReportedByUserID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(typeof(User).ToString(), "ReportedByUserID", request.ReportedByUserID);
        }

        // Check if resolvedByUserID exists
        if (request.ResolvedByUserID != null)
        {
            var resolvedByUser = await _userRepository.GetByIdAsync(request.ResolvedByUserID);
            if (resolvedByUser == null)
            {
                var errorMessage = $"ResolvedByUserID not found: {request.ResolvedByUserID}";
                _logger.LogError(errorMessage);
                throw new EntityNotFoundException(typeof(User).ToString(), "ResolvedByUserID", request.ResolvedByUserID);
            }
        }
    }
}