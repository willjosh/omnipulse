using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;

using AutoMapper;

using MediatR;

namespace Application.Features.WorkOrders.Command.CreateWorkOrder;

public class CreateWorkOrderCommandHandler : IRequestHandler<CreateWorkOrderCommand, int>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<CreateWorkOrderCommandHandler> _logger;

    public CreateWorkOrderCommandHandler(IWorkOrderRepository workOrderRepository, IAppLogger<CreateWorkOrderCommandHandler> logger, IMapper mapper)
    {
        _workOrderRepository = workOrderRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public Task<int> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}