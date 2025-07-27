using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;



namespace Application.Features.Issues.Command.CreateIssue;

public class CreateIssueCommandHandler : IRequestHandler<CreateIssueCommand, int>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<CreateIssueCommandHandler> _logger;
    private readonly IValidator<CreateIssueCommand> _validator;

    public CreateIssueCommandHandler(IIssueRepository issueRepository, IVehicleRepository vehicleRepository, IUserRepository userRepository, IMapper mapper, IAppLogger<CreateIssueCommandHandler> logger, IValidator<CreateIssueCommand> validator)
    {
        _issueRepository = issueRepository;
        _vehicleRepository = vehicleRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<int> Handle(CreateIssueCommand request, CancellationToken cancellationToken)
    {
        // validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"CreateIssueCommand - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // map request to issue domain entity
        var issue = _mapper.Map<Issue>(request);

        // validate business rules
        await ValidateBusinessRuleAsync(issue);

        // add new issue
        var createdIssue = await _issueRepository.AddAsync(issue);
        await _issueRepository.SaveChangesAsync();

        _logger.LogInformation($"CreateIssueCommand - Issue created successfully with ID: {createdIssue.ID}");
        return createdIssue.ID;
    }

    private async Task ValidateBusinessRuleAsync(Issue issue)
    {
        // check if vehicle exists
        var vehicle = await _vehicleRepository.GetByIdAsync(issue.VehicleID);
        if (vehicle == null)
        {
            _logger.LogWarning($"CreateIssueCommand - Vehicle not found: {issue.VehicleID}");
            throw new EntityNotFoundException(typeof(Vehicle).ToString(), "VehicleID", issue.VehicleID.ToString());
        }

        // check if user exists
        var user = await _userRepository.GetByIdAsync(issue.ReportedByUserID);
        if (user == null)
        {
            _logger.LogWarning($"CreateIssueCommand - User not found: {issue.ReportedByUserID}");
            throw new EntityNotFoundException(typeof(User).ToString(), "ReportedByUserID", issue.ReportedByUserID);
        }
    }
}