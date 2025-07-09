using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.WorkOrderLineItem.Query.GetWorkOrderLineItemDetail;

using AutoMapper;

using Domain.Entities;

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

    public async Task<GetWorkOrderDetailDTO> Handle(GetWorkOrderDetailQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"GetWorkOrderDetailQuery for WorkOrderID: {request.ID}");

        var workOrder = await _workOrderRepository.GetWorkOrderWithDetailsAsync(request.ID);
        if (workOrder == null)
        {
            _logger.LogError($"WorkOrder with ID {request.ID} not found.");
            throw new EntityNotFoundException(typeof(WorkOrder).ToString(), "WorkOrderID", request.ID.ToString());
        }

        // Get work order line items
        var workOrderLineItems = await _workOrderLineItemRepository.GetByWorkOrderIdAsync(request.ID);

        // Handle null line items
        List<WorkOrderLineItemDetailDTO> lineItemDtos;

        if (workOrderLineItems == null || !workOrderLineItems.Any())
        {
            _logger.LogInformation($"No line items found for WorkOrder with ID {request.ID}");
            lineItemDtos = new List<WorkOrderLineItemDetailDTO>();
        }
        else
        {
            // Calculate total cost for each line item
            workOrderLineItems.ForEach(item => item.CalculateTotalCost());

            // Map line items to DTO
            lineItemDtos = workOrderLineItems.Select(item =>
            {
                var lineItemDto = _mapper.Map<WorkOrderLineItemDetailDTO>(item);
                lineItemDto.LaborCost = item.CalculateLaborCost();
                lineItemDto.ItemCost = item.CalculateItemCost();
                lineItemDto.SubTotal = item.TotalCost;
                return lineItemDto;
            }).ToList();
        }

        // Map to GetWorkOrderDetailDTO
        var workOrderDetailDto = _mapper.Map<GetWorkOrderDetailDTO>(workOrder);
        workOrderDetailDto.WorkOrderLineItems = lineItemDtos;

        // Calculate work order level totals
        workOrderDetailDto.TotalLaborCost = lineItemDtos.Sum(li => li.LaborCost);
        workOrderDetailDto.TotalItemCost = lineItemDtos.Sum(li => li.ItemCost);
        workOrderDetailDto.TotalCost = lineItemDtos.Sum(li => li.SubTotal);

        _logger.LogInformation($"Successfully retrieved WorkOrder with ID {request.ID}");

        return workOrderDetailDto;
    }
}