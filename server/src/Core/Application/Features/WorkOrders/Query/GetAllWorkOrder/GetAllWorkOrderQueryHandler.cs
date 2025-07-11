using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.WorkOrderLineItem.Query.GetWorkOrderLineItemDetail;
using Application.Features.WorkOrders.Query.GetWorkOrderDetail;
using Application.Models.PaginationModels;

using AutoMapper;

using Domain.Entities;
using Domain.Services;

using FluentValidation;

using MediatR;

namespace Application.Features.WorkOrders.Query.GetAllWorkOrder;

public class GetAllWorkOrderQueryHandler : IRequestHandler<GetAllWorkOrderQuery, PagedResult<GetWorkOrderDetailDTO>>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IWorkOrderLineItemRepository _workOrderLineItemRepository;
    private readonly IAppLogger<GetAllWorkOrderQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IValidator<GetAllWorkOrderQuery> _validator;

    public GetAllWorkOrderQueryHandler(
        IWorkOrderRepository workOrderRepository,
        IWorkOrderLineItemRepository workOrderLineItemRepository,
        IAppLogger<GetAllWorkOrderQueryHandler> logger,
        IMapper mapper,
        IValidator<GetAllWorkOrderQuery> validator
    )
    {
        _workOrderRepository = workOrderRepository;
        _workOrderLineItemRepository = workOrderLineItemRepository;
        _logger = logger;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<PagedResult<GetWorkOrderDetailDTO>> Handle(GetAllWorkOrderQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllWorkOrderQuery");
        // Validate the request
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"GetAllWorkOrder - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Get all work orders from the repository
        var result = await _workOrderRepository.GetAllWorkOrderPagedAsync(request.Parameters);

        // if no workOrder return empty result
        if (!result.Items.Any())
        {
            return new PagedResult<GetWorkOrderDetailDTO>
            {
                Items = [],
                TotalCount = result.TotalCount,
                PageNumber = request.Parameters.PageNumber,
                PageSize = request.Parameters.PageSize
            };
        }

        var workOrderIds = result.Items.Select(wo => wo.ID).ToList();
        var allLineItems = await _workOrderLineItemRepository.GetByWorkOrderIdsAsync(workOrderIds);

        // Group line items by WorkOrder ID for efficient lookup
        var lineItemsLookup = allLineItems.GroupBy(li => li.WorkOrderID).ToDictionary(g => g.Key, g => g.ToList());

        // Map the work orders to DTOs
        var workOrderDtos = result.Items.Select(workOrder => CreateWorkOrderDetailDto(workOrder, lineItemsLookup)).ToList();
        
        return new PagedResult<GetWorkOrderDetailDTO>
        {
            Items = workOrderDtos,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

    private GetWorkOrderDetailDTO CreateWorkOrderDetailDto(WorkOrder workOrder, Dictionary<int, List<Domain.Entities.WorkOrderLineItem>> lineItemsLookup)
    {
        // Get line items for this work order (or empty list if none)
        var lineItems = lineItemsLookup.TryGetValue(workOrder.ID, out var items) ? items : new List<Domain.Entities.WorkOrderLineItem>();

        // Calculate totals
        lineItems.EnsureTotalCalculated();
        var workOrderLineItemTotals = lineItems.CalculateTotals();

        // Map line items to DTO
        var lineItemDtos = MapLineItemsToDto(lineItems);

        if (!lineItems.Any())
        {
            _logger.LogDebug("No line items found for WorkOrder with ID {WorkOrderId}", workOrder.ID);
        }

        // Map to GetWorkOrderDetailDTO
        var workOrderDetailDto = _mapper.Map<GetWorkOrderDetailDTO>(workOrder);
        workOrderDetailDto.WorkOrderLineItems = lineItemDtos;

        // Set calculated totals
        workOrderDetailDto.TotalLaborCost = workOrderLineItemTotals.TotalLaborCost;
        workOrderDetailDto.TotalItemCost = workOrderLineItemTotals.TotalItemCost;
        workOrderDetailDto.TotalCost = workOrderLineItemTotals.TotalCost;
        
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
