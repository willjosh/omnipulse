using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.WorkOrderLineItem.Models;

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
    private readonly IWorkOrderLineItemRepository _workOrderLineItemRepository; // Add this
    private readonly IInventoryItemRepository _inventoryItemRepository; // Add this
    private readonly IServiceTaskRepository _serviceTaskRepository; // Add this
    private readonly IMapper _mapper;
    private readonly IAppLogger<CreateWorkOrderCommandHandler> _logger;
    private readonly IValidator<CreateWorkOrderCommand> _validator;

    public CreateWorkOrderCommandHandler(IWorkOrderRepository workOrderRepository, IUserRepository userRepository, IVehicleRepository vehicleRepository, IServiceReminderRepository serviceReminder, IIssueRepository issueRepository, IWorkOrderIssueRepository workOrderIssueRepository,
    IWorkOrderLineItemRepository workOrderLineItemRepository, IInventoryItemRepository inventoryItemRepository, IServiceTaskRepository serviceTaskRepository,
    IAppLogger<CreateWorkOrderCommandHandler> logger, IMapper mapper, IValidator<CreateWorkOrderCommand> validator)
    {
        _workOrderRepository = workOrderRepository;
        _userRepository = userRepository;
        _vehicleRepository = vehicleRepository;
        _serviceReminderRepository = serviceReminder;
        _issueRepository = issueRepository;
        _workOrderIssueRepository = workOrderIssueRepository;
        _workOrderLineItemRepository = workOrderLineItemRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _serviceTaskRepository = serviceTaskRepository;
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
        await ValidateBusinessRuleAsync(workOrder, request.IssueIdList, request.WorkOrderLineItems);

        // add new work order
        var newWorkOrder = await _workOrderRepository.AddAsync(workOrder);
        _logger.LogInformation($"Work order created with ID: {newWorkOrder.ID}");

        // save the new work order
        await _workOrderRepository.SaveChangesAsync();

        // create workOrderLineItems and WorkOrderIssues
        await CreateRelatedEntitiesAsync(newWorkOrder.ID, request);

        await _workOrderRepository.SaveChangesAsync();
        _logger.LogInformation($"Work order created successfully with ID: {newWorkOrder.ID}");
        return newWorkOrder.ID;
    }

    private async Task ValidateBusinessRuleAsync(WorkOrder workOrder, List<int>? idsList, List<CreateWorkOrderLineItemDTO>? workOrderLineItems)
    {
        // Check if the vehicle exists
        if (!await _vehicleRepository.ExistsAsync(workOrder.VehicleID))
        {
            var errorMessage = $"Vehicle ID not found: {workOrder.VehicleID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(nameof(Vehicle), "VehicleID", workOrder.VehicleID.ToString());
        }

        // Check if the user exists
        if (!await _userRepository.ExistsAsync(workOrder.AssignedToUserID))
        {
            var errorMessage = $"User not found: {workOrder.AssignedToUserID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(nameof(User), "AssignedToUserID", workOrder.AssignedToUserID);
        }

        // Validate issues if provided
        if (idsList?.Count > 0)
        {
            if (!await _issueRepository.AllExistAsync(idsList))
            {
                var errorMessage = $"One or more issues not found: {string.Join(", ", idsList)}";
                _logger.LogError(errorMessage);
                throw new EntityNotFoundException(nameof(Issue), "IssueIds", string.Join(", ", idsList));
            }
        }

        // Validate work order line items if provided
        if (workOrderLineItems?.Count > 0)
        {
            // Batch validate inventory items
            var inventoryItemIds = workOrderLineItems
                .Where(item => item.InventoryItemID.HasValue)
                .Select(item => item.InventoryItemID!.Value)
                .Distinct()
                .ToList();

            if (inventoryItemIds.Count > 0 && !await _inventoryItemRepository.AllExistAsync(inventoryItemIds))
            {
                var errorMessage = $"One or more inventory items not found: {string.Join(", ", inventoryItemIds)}";
                _logger.LogError(errorMessage);
                throw new EntityNotFoundException(nameof(InventoryItem), "InventoryItemIds", string.Join(", ", inventoryItemIds));
            }

            // Batch validate service tasks
            var serviceTaskIds = workOrderLineItems
                .Select(item => item.ServiceTaskID)
                .Distinct()
                .ToList();

            if (!await _serviceTaskRepository.AllExistAsync(serviceTaskIds))
            {
                var errorMessage = $"One or more service tasks not found: {string.Join(", ", serviceTaskIds)}";
                _logger.LogError(errorMessage);
                throw new EntityNotFoundException(nameof(ServiceTask), "ServiceTaskIds", string.Join(", ", serviceTaskIds));
            }

            // Batch validate assigned users in line items
            var assignedUserIds = workOrderLineItems
                .Where(item => !string.IsNullOrEmpty(item.AssignedToUserID))
                .Select(item => item.AssignedToUserID!)
                .Distinct()
                .ToList();

            foreach (var userId in assignedUserIds)
            {
                if (!await _userRepository.ExistsAsync(userId))
                {
                    var errorMessage = $"User not found: {userId}";
                    _logger.LogError(errorMessage);
                    throw new EntityNotFoundException(nameof(User), "AssignedToUserID", userId);
                }
            }
        }
    }


    private async Task CreateRelatedEntitiesAsync(int workOrderID, CreateWorkOrderCommand request)
    {
        // create work order line items
        if (request.WorkOrderLineItems?.Count > 0)
        {
            await CreateWorkOrderLineItemsAsync(workOrderID, request.WorkOrderLineItems);
            _logger.LogInformation($"Created {request.WorkOrderLineItems.Count} work order line items");
        }

        // create Work Order Issues
        if (request.IssueIdList?.Count > 0)
        {
            await CreateWorkOrderIssueAsync(workOrderID, request.IssueIdList);
            _logger.LogInformation($"Created {request.IssueIdList.Count} work order issue relationships");
        }
    }

    private async Task CreateWorkOrderLineItemsAsync(int workOrderID, List<CreateWorkOrderLineItemDTO> lineItemCommands)
    {
        // Map the line items from DTO to domain entity
        var lineItems = lineItemCommands.Select(dto =>
        {
            var lineItem = _mapper.Map<Domain.Entities.WorkOrderLineItem>(dto);
            lineItem.WorkOrderID = workOrderID; // Set the WorkOrderID
            lineItem.CalculateTotalCost(); // Calculate the total cost
            return lineItem;
        }).ToList();

        await _workOrderLineItemRepository.AddRangeAsync(lineItems);
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