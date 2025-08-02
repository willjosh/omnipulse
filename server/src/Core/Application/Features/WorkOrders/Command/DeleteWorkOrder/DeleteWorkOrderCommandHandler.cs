using System;

using Application.Contracts.Persistence;
using Application.Exceptions;

using Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Application.Features.WorkOrders.Command.DeleteWorkOrder;

public class DeleteWorkOrderCommandHandler : IRequestHandler<DeleteWorkOrderCommand, int>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly ILogger<DeleteWorkOrderCommandHandler> _logger;

    public DeleteWorkOrderCommandHandler(IWorkOrderRepository workOrderRepository, ILogger<DeleteWorkOrderCommandHandler> logger)
    {
        _workOrderRepository = workOrderRepository;
        _logger = logger;
    }

    public async Task<int> Handle(DeleteWorkOrderCommand request, CancellationToken cancellationToken)
    {
        // validate the workOrder exists
        var workOrder = await _workOrderRepository.GetByIdAsync(request.ID);

        if (workOrder == null)
        {
            _logger.LogError($"WorkOrder with ID {request.ID} not found.");
            throw new EntityNotFoundException(typeof(WorkOrder).ToString(), "ID", request.ID.ToString());
        }

        // delete the workOrder
        _workOrderRepository.Delete(workOrder);

        // save changes
        await _workOrderRepository.SaveChangesAsync();
        _logger.LogInformation($"WorkOrder with ID: {request.ID} deleted");

        return request.ID;
    }
}