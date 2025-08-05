using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Features.WorkOrders.Query.GetWorkOrderDetail;

using MediatR;

namespace Application.Features.WorkOrders.Query.GetWorkOrderStatusData;

public class GetWorkOrderStatusDataQueryHandler : IRequestHandler<GetWorkOrderStatusDataQuery, GetWorkOrderStatusDataDTO>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IAppLogger<GetWorkOrderQueryHandler> _logger;
    public GetWorkOrderStatusDataQueryHandler(
        IWorkOrderRepository workOrderRepository,
        IAppLogger<GetWorkOrderQueryHandler> logger)
    {
        _workOrderRepository = workOrderRepository;
        _logger = logger;
    }

    public async Task<GetWorkOrderStatusDataDTO> Handle(GetWorkOrderStatusDataQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetWorkOrderStatusDataQuery");
        var createdCountTask = await _workOrderRepository.GetAllCreatedWorkOrdersCountAsync();
        var inProgressCountTask = await _workOrderRepository.GetAllInProgressWorkOrdersCountAsync();

        var result = new GetWorkOrderStatusDataDTO
        {
            CreatedCount = createdCountTask,
            InProgressCount = inProgressCountTask
        };

        _logger.LogInformation("GetWorkOrderStatusDataQuery handled successfully");
        return result;
    }
}