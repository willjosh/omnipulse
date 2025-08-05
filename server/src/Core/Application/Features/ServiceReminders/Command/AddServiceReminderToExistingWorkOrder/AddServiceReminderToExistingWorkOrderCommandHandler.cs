using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;

using MediatR;

namespace Application.Features.ServiceReminders.Command.AddServiceReminderToExistingWorkOrder;

public sealed class AddServiceReminderToExistingWorkOrderCommandHandler : IRequestHandler<AddServiceReminderToExistingWorkOrderCommand, int>
{
    private readonly IServiceReminderRepository _serviceReminderRepository;
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IValidator<AddServiceReminderToExistingWorkOrderCommand> _validator;
    private readonly IAppLogger<AddServiceReminderToExistingWorkOrderCommandHandler> _logger;

    public AddServiceReminderToExistingWorkOrderCommandHandler(
        IServiceReminderRepository serviceReminderRepository,
        IWorkOrderRepository workOrderRepository,
        IValidator<AddServiceReminderToExistingWorkOrderCommand> validator,
        IAppLogger<AddServiceReminderToExistingWorkOrderCommandHandler> logger)
    {
        _serviceReminderRepository = serviceReminderRepository;
        _workOrderRepository = workOrderRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<int> Handle(AddServiceReminderToExistingWorkOrderCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(AddServiceReminderToExistingWorkOrderCommand)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Get service reminder
        var serviceReminder = await _serviceReminderRepository.GetServiceReminderWithDetailsAsync(request.ServiceReminderID);
        if (serviceReminder == null)
        {
            _logger.LogError($"ServiceReminder with ID {request.ServiceReminderID} not found.");
            throw new EntityNotFoundException(nameof(ServiceReminder), nameof(ServiceReminder.ID), request.ServiceReminderID.ToString());
        }

        // Validate service reminder is not already linked to a work order
        if (serviceReminder.WorkOrderID.HasValue)
        {
            _logger.LogError($"ServiceReminder {request.ServiceReminderID} is already linked to WorkOrder {serviceReminder.WorkOrderID.Value}.");
            throw new BadRequestException($"Service reminder is already linked to a work order.");
        }

        // Validate service reminder is in a valid state
        if (serviceReminder.Status == ServiceReminderStatusEnum.COMPLETED ||
            serviceReminder.Status == ServiceReminderStatusEnum.CANCELLED)
        {
            _logger.LogError($"ServiceReminder {request.ServiceReminderID} is in invalid state: {serviceReminder.Status}.");
            throw new BadRequestException($"Cannot link service reminder in {serviceReminder.Status} state to a work order.");
        }

        // Validate work order exists
        var workOrder = await _workOrderRepository.GetByIdAsync(request.WorkOrderID);
        if (workOrder == null)
        {
            _logger.LogError($"WorkOrder with ID {request.WorkOrderID} not found.");
            throw new EntityNotFoundException(nameof(WorkOrder), nameof(WorkOrder.ID), request.WorkOrderID.ToString());
        }

        if (workOrder.Status == WorkOrderStatusEnum.COMPLETED ||
            workOrder.Status == WorkOrderStatusEnum.CANCELLED)
            throw new BadRequestException($"Cannot link reminder to a work order with {workOrder.Status} status.");

        // Link service reminder to work order
        serviceReminder.WorkOrderID = request.WorkOrderID;
        serviceReminder.WorkOrder = workOrder;

        await _serviceReminderRepository.SaveChangesAsync();

        _logger.LogInformation($"Service reminder {request.ServiceReminderID} successfully linked to work order {request.WorkOrderID}.");

        return request.WorkOrderID;
    }
}