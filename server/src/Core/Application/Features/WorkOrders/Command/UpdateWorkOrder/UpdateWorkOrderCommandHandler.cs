using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using FluentValidation;

using MediatR;

namespace Application.Features.WorkOrders.Command.UpdateWorkOrder;

public class UpdateWorkOrderCommandHandler : IRequestHandler<UpdateWorkOrderCommand, int>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IServiceReminderRepository _serviceReminderRepository;
    private readonly IIssueRepository _issueRepository;
    private readonly IWorkOrderIssueRepository _workOrderIssueRepository;
    private readonly IWorkOrderLineItemRepository _workOrderLineItemRepository;
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IServiceTaskRepository _serviceTaskRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<UpdateWorkOrderCommandHandler> _logger;
    private readonly IValidator<UpdateWorkOrderCommand> _validator;

    public UpdateWorkOrderCommandHandler(
        IWorkOrderRepository workOrderRepository,
        IUserRepository userRepository,
        IVehicleRepository vehicleRepository,
        IServiceReminderRepository serviceReminderRepository,
        IIssueRepository issueRepository,
        IWorkOrderIssueRepository workOrderIssueRepository,
        IWorkOrderLineItemRepository workOrderLineItemRepository,
        IInventoryItemRepository inventoryItemRepository,
        IServiceTaskRepository serviceTaskRepository,
        IAppLogger<UpdateWorkOrderCommandHandler> logger,
        IMapper mapper,
        IValidator<UpdateWorkOrderCommand> validator)
    {
        _workOrderRepository = workOrderRepository;
        _userRepository = userRepository;
        _vehicleRepository = vehicleRepository;
        _serviceReminderRepository = serviceReminderRepository;
        _issueRepository = issueRepository;
        _workOrderIssueRepository = workOrderIssueRepository;
        _workOrderLineItemRepository = workOrderLineItemRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _serviceTaskRepository = serviceTaskRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<int> Handle(UpdateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        // Validate the request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"UpdateWorkOrder - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Retrieve the existing work order
        var workOrder = await _workOrderRepository.GetByIdAsync(request.WorkOrderID);
        if (workOrder == null)
        {
            _logger.LogError($"Work order not found: {request.WorkOrderID}");
            throw new EntityNotFoundException(nameof(Domain.Entities.WorkOrder), "WorkOrderID", request.WorkOrderID.ToString());
        }

        // Validate business rules (vehicle, user, issues, etc.)
        await ValidateBusinessRuleAsync(request);

        // Use AutoMapper to map request onto the existing entity (in-place)
        _mapper.Map(request, workOrder);

        // Update related entities (issues, line items)
        await UpdateRelatedEntitiesAsync(workOrder.ID, request);

        _workOrderRepository.Update(workOrder);
        await _workOrderRepository.SaveChangesAsync();

        _logger.LogInformation($"Work order updated successfully with ID: {workOrder.ID}");
        return workOrder.ID;
    }

    // Helper: Validate business rules (vehicle, user, issues, etc.)
    private async Task ValidateBusinessRuleAsync(UpdateWorkOrderCommand request)
    {
        // Check if the vehicle exists
        if (!await _vehicleRepository.ExistsAsync(request.VehicleID))
        {
            var errorMessage = $"Vehicle ID not found: {request.VehicleID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(nameof(Domain.Entities.Vehicle), "VehicleID", request.VehicleID.ToString());
        }

        // Check if the user exists
        if (!await _userRepository.ExistsAsync(request.AssignedToUserID))
        {
            var errorMessage = $"User not found: {request.AssignedToUserID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(nameof(Domain.Entities.User), "AssignedToUserID", request.AssignedToUserID);
        }

        // Validate issues if provided
        if (request.IssueIdList?.Count > 0)
        {
            if (!await _issueRepository.AllExistAsync(request.IssueIdList))
            {
                var errorMessage = $"One or more issues not found: {string.Join(", ", request.IssueIdList)}";
                _logger.LogError(errorMessage);
                throw new EntityNotFoundException(nameof(Domain.Entities.Issue), "IssueIds", string.Join(", ", request.IssueIdList));
            }
        }

        // Validate work order line items if provided
        if (request.WorkOrderLineItems?.Count > 0)
        {
            // Batch validate inventory items
            var inventoryItemIds = request.WorkOrderLineItems
                .Where(item => item.InventoryItemID.HasValue)
                .Select(item => item.InventoryItemID!.Value)
                .Distinct()
                .ToList();

            if (inventoryItemIds.Count > 0 && !await _inventoryItemRepository.AllExistAsync(inventoryItemIds))
            {
                var errorMessage = $"One or more inventory items not found: {string.Join(", ", inventoryItemIds)}";
                _logger.LogError(errorMessage);
                throw new EntityNotFoundException(nameof(Domain.Entities.InventoryItem), "InventoryItemIds", string.Join(", ", inventoryItemIds));
            }

            // Batch validate service tasks
            var serviceTaskIds = request.WorkOrderLineItems
                .Select(item => item.ServiceTaskID)
                .Distinct()
                .ToList();

            if (!await _serviceTaskRepository.AllExistAsync(serviceTaskIds))
            {
                var errorMessage = $"One or more service tasks not found: {string.Join(", ", serviceTaskIds)}";
                _logger.LogError(errorMessage);
                throw new EntityNotFoundException(nameof(Domain.Entities.ServiceTask), "ServiceTaskIds", string.Join(", ", serviceTaskIds));
            }

            // Batch validate assigned users in line items
            var assignedUserIds = request.WorkOrderLineItems
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
                    throw new EntityNotFoundException(nameof(Domain.Entities.User), "AssignedToUserID", userId);
                }
            }
        }
    }

    // Helper: Update related entities (issues, line items)
    private async Task UpdateRelatedEntitiesAsync(int workOrderID, UpdateWorkOrderCommand request)
    {
        // Update work order line items
        if (request.WorkOrderLineItems != null)
        {
            // Remove existing line items
            await _workOrderLineItemRepository.DeleteByWorkOrderIdAsync(workOrderID);

            // Add new line items
            if (request.WorkOrderLineItems.Count > 0)
            {
                var lineItems = request.WorkOrderLineItems.Select(dto =>
                {
                    var lineItem = _mapper.Map<Domain.Entities.WorkOrderLineItem>(dto);
                    lineItem.WorkOrderID = workOrderID;
                    lineItem.CalculateTotalCost();
                    return lineItem;
                }).ToList();

                await _workOrderLineItemRepository.AddRangeAsync(lineItems);
            }
        }

        // Update work order issues
        if (request.IssueIdList != null)
        {
            // Remove existing issues
            await _workOrderIssueRepository.DeleteByWorkOrderIdAsync(workOrderID);

            // Add new issues
            if (request.IssueIdList.Count > 0)
            {
                var listOfWorkOrderIssue = request.IssueIdList.Select(
                    issueID => new Domain.Entities.WorkOrderIssue
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
    }
}