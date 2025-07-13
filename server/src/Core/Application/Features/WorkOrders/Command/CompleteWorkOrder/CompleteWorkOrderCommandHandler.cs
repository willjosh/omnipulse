using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Services;

using MediatR;

namespace Application.Features.WorkOrders.Command.CompleteWorkOrder;

public class CompleteWorkOrderCommandHandler : IRequestHandler<CompleteWorkOrderCommand, int>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IMaintenanceHistoryRepository _maintenanceHistoryRepository;
    private readonly IAppLogger<CompleteWorkOrderCommandHandler> _logger;
    private readonly IMapper _mapper;

    public CompleteWorkOrderCommandHandler(IWorkOrderRepository workOrderRepository, IMaintenanceHistoryRepository maintenanceHistoryRepository, IAppLogger<CompleteWorkOrderCommandHandler> logger, IMapper mapper)
    {
        _workOrderRepository = workOrderRepository;
        _maintenanceHistoryRepository = maintenanceHistoryRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> Handle(CompleteWorkOrderCommand request, CancellationToken cancellationToken)
    {
        // check if the work order exists
        var workOrder = await _workOrderRepository.GetWorkOrderWithDetailsAsync(request.ID);
        if (workOrder == null)
        {
            _logger.LogError($"Work order with ID {request.ID} not found.");
            throw new EntityNotFoundException(typeof(WorkOrder).ToString(), "WorkOrderID", request.ID.ToString());
        }

        // check if the work order is already completed
        if (workOrder.Status == WorkOrderStatusEnum.COMPLETED)
        {
            _logger.LogWarning($"Work order with ID {request.ID} is already completed.");
            throw new WorkOrderAlreadyCompletedException(request.ID);
        }

        // check if required fields are filled
        if (workOrder.IsReadyForCompletion() == false)
        {
            _logger.LogWarning($"Work order with ID {request.ID} is not ready for completion.");
            throw new IncompleteWorkOrderException(request.ID);
        }

        // update the work order status to completed
        workOrder.Status = WorkOrderStatusEnum.COMPLETED;
        _workOrderRepository.Update(workOrder);

        // create a maintenance history entry
        var maintenanceHistory = _mapper.Map<MaintenanceHistory>(workOrder);
        await _maintenanceHistoryRepository.AddAsync(maintenanceHistory);

        await _maintenanceHistoryRepository.SaveChangesAsync();

        // return the updated work order id
        return workOrder.ID;
    }
}