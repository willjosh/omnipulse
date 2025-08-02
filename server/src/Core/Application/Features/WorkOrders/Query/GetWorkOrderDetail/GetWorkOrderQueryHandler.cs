using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.WorkOrderLineItem.Query.GetWorkOrderLineItemDetail;

using AutoMapper;

using Domain.Entities;
using Domain.Services;

using MediatR;

namespace Application.Features.WorkOrders.Query.GetWorkOrderDetail;

public class GetWorkOrderQueryHandler : IRequestHandler<GetWorkOrderDetailQuery, GetWorkOrderDetailDTO>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IWorkOrderLineItemRepository _workOrderLineItemRepository;
    private readonly IWorkOrderIssueRepository _workOrderIssueRepository;
    private readonly IAppLogger<GetWorkOrderQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetWorkOrderQueryHandler(
        IWorkOrderRepository workOrderRepository,
        IWorkOrderLineItemRepository workOrderLineItemRepository,
        IWorkOrderIssueRepository workOrderIssueRepository,
        IAppLogger<GetWorkOrderQueryHandler> logger,
        IMapper mapper)
    {
        _workOrderRepository = workOrderRepository;
        _workOrderLineItemRepository = workOrderLineItemRepository;
        _workOrderIssueRepository = workOrderIssueRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<GetWorkOrderDetailDTO> Handle(GetWorkOrderDetailQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetWorkOrderDetailQuery for WorkOrderID: {WorkOrderId}", request.ID);

        var workOrder = await _workOrderRepository.GetWorkOrderWithDetailsAsync(request.ID);
        if (workOrder == null)
        {
            _logger.LogError("WorkOrder with ID {WorkOrderId} not found", request.ID);
            throw new EntityNotFoundException(typeof(WorkOrder).ToString(), "WorkOrderID", request.ID.ToString());
        }

        var workOrderIssues = await _workOrderIssueRepository.GetByWorkOrderIDAsync(request.ID);
        var workOrderLineItems = await _workOrderLineItemRepository.GetByWorkOrderIdAsync(request.ID);

        var lineItems = workOrderLineItems ?? new List<Domain.Entities.WorkOrderLineItem>();

        // ALWAYS calculate totals (even for empty list)
        lineItems.EnsureTotalCalculated();
        var workOrderLineItemTotals = lineItems.CalculateTotals();

        // Map line items to DTO with calculated values
        var lineItemDtos = MapLineItemsToDto(lineItems);

        if (!lineItems.Any())
        {
            _logger.LogInformation("No line items found for WorkOrder with ID {WorkOrderId}", request.ID);
        }

        // Map to GetWorkOrderDetailDTO
        var workOrderDetailDto = _mapper.Map<GetWorkOrderDetailDTO>(workOrder);
        workOrderDetailDto.WorkOrderLineItems = lineItemDtos;

        // Set calculated totals
        workOrderDetailDto.TotalLaborCost = workOrderLineItemTotals.TotalLaborCost;
        workOrderDetailDto.TotalItemCost = workOrderLineItemTotals.TotalItemCost;
        workOrderDetailDto.TotalCost = workOrderLineItemTotals.TotalCost;
        workOrderDetailDto.IssueIDs = [.. workOrderIssues.Select(woi => woi.IssueID)];

        _logger.LogInformation("Successfully retrieved WorkOrder with ID {WorkOrderId}", request.ID);
        return workOrderDetailDto;
    }

    private List<WorkOrderLineItemDetailDTO> MapLineItemsToDto(List<Domain.Entities.WorkOrderLineItem> lineItems)
    {
        return lineItems.Select(item =>
        {
            // Use AutoMapper for basic mapping
            var lineItemDto = _mapper.Map<WorkOrderLineItemDetailDTO>(item);

            // Add calculated values that AutoMapper can't handle
            lineItemDto.LaborCost = item.CalculateLaborCost();
            lineItemDto.ItemCost = item.CalculateItemCost();
            lineItemDto.SubTotal = item.TotalCost;

            return lineItemDto;
        }).ToList();
    }
}