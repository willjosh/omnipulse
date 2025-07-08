using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.WorkOrders.Command.CreateWorkOrder;

public class CreateWorkOrderCommandHandler : IRequestHandler<CreateWorkOrderCommand, int>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IServiceReminderRepository _serviceReminderRepository;
    private readonly IIssueRepository _issueRepository;
    private readonly IWorkOrderIssueRepository _workOrderIssueRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<CreateWorkOrderCommandHandler> _logger;
    private readonly IValidator<CreateWorkOrderCommand> _validator;

    public CreateWorkOrderCommandHandler(IWorkOrderRepository workOrderRepository, IUserRepository userRepository, IVehicleRepository vehicleRepository, IServiceReminderRepository serviceReminder, IIssueRepository issueRepository, IWorkOrderIssueRepository workOrderIssueRepository, IAppLogger<CreateWorkOrderCommandHandler> logger, IMapper mapper, IValidator<CreateWorkOrderCommand> validator)
    {
        _workOrderRepository = workOrderRepository;
        _userRepository = userRepository;
        _vehicleRepository = vehicleRepository;
        _serviceReminderRepository = serviceReminder;
        _issueRepository = issueRepository;
        _workOrderIssueRepository = workOrderIssueRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<int> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        // validate the request 
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"CreateWorkOrder - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // map request to work order domain entity
        var workOrder = _mapper.Map<WorkOrder>(request);

        // validate business rules
        await ValidateBusinessRuleAsync(workOrder, request.IssueIdList);

        // add new work order
        var newWorkOrder = await _workOrderRepository.AddAsync(workOrder);
        _logger.LogInformation($"Work order created with ID: {newWorkOrder.ID}");

        if (request.IssueIdList?.Any() == true)
        {
            await CreateWorkOrderIssueAsync(newWorkOrder.ID, request.IssueIdList);
            _logger.LogInformation($"Created {request.IssueIdList.Count} work order issue relationships");
        }

        await _workOrderRepository.SaveChangesAsync();
        _logger.LogInformation($"Work order created successfully with ID: {newWorkOrder.ID}");
        return newWorkOrder.ID;
    }

    private async Task ValidateBusinessRuleAsync(WorkOrder workOrder, List<int>? IdsList)
    {
        // check if the vehicle exists
        if (await _vehicleRepository.ExistsAsync(workOrder.VehicleID) == false)
        {
            var errorMessage = $"Vehicle ID not found: {workOrder.VehicleID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(typeof(Vehicle).ToString(), "VehicleID", workOrder.VehicleID.ToString());
        }

        // check if the user exists
        if (!await _userRepository.ExistsAsync(workOrder.AssignedToUserID))
        {
            var errorMessage = $"User not found: {workOrder.AssignedToUserID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(nameof(User), "AssignedToUserID", workOrder.AssignedToUserID);
        }

        // check if the service reminder exists
        if (await _serviceReminderRepository.ExistsAsync(workOrder.ServiceReminderID) == false)
        {
            var errorMessage = $"Service reminder ID not found: {workOrder.ServiceReminderID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(typeof(ServiceReminder).ToString(), "ServiceReminderID", workOrder.ServiceReminderID.ToString());
        }

        if (IdsList?.Any() == true)
        {
            if (!await _issueRepository.AllExistAsync(IdsList))
            {
                var errorMessage = $"One or more issues not found: {string.Join(", ", IdsList)}";
                _logger.LogError(errorMessage);
                throw new EntityNotFoundException(nameof(Issue), "IssueIds", string.Join(", ", IdsList));
            }
        }
    }

    private async Task CreateWorkOrderIssueAsync(int workOrderID, List<int> issueIdList)
    {
        var listOfWorkOrderIssue = issueIdList.Select(
            issueID => new WorkOrderIssue
            {
                WorkOrderID = workOrderID,
                IssueID = issueID,
                AssignedDate = DateTime.UtcNow,
                WorkOrder = null!,
                Issue = null!
            }
        ).ToList();

        await _workOrderIssueRepository.AddRangeAsync(listOfWorkOrderIssue);
    }
}