using Application.Contracts.Logger;
using Application.Contracts.Persistence;

using AutoMapper;

using MediatR;

namespace Application.Features.WorkOrders.Query.GetWorkOrderDetail;

public class GetWorkOrderQueryHandler : IRequestHandler<GetWorkOrderDetailQuery, GetWorkOrderDetailDTO>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IWorkOrderLineItemRepository _workOrderLineItemRepository;
    private readonly IAppLogger<GetWorkOrderQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetWorkOrderQueryHandler(IWorkOrderRepository workOrderRepository, IWorkOrderLineItemRepository workOrderLineItemRepository, IAppLogger<GetWorkOrderQueryHandler> logger, IMapper mapper)
    {
        _workOrderRepository = workOrderRepository;
        _workOrderLineItemRepository = workOrderLineItemRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public Task<GetWorkOrderDetailDTO> Handle(GetWorkOrderDetailQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}