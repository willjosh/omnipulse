using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;

using AutoMapper;

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

    public Task<int> Handle(CompleteWorkOrderCommand request, CancellationToken cancellationToken)
    {
        // check if the work order exists
        // check if the work order is already completed
        // check if required fields are filled
        // update the work order status to completed
        // create a maintenance history entry
        // return the updated work order id
        throw new NotImplementedException();
    }
}