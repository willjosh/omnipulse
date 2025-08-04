using System.Linq;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Models.PaginationModels;

using AutoMapper;

using Domain.Services;

using FluentValidation;

using MediatR;

namespace Application.Features.MaintenanceHistories.Query.GetAllMaintenanceHistories;

public class GetAllMaintenanceHistoriesQueryHandler : IRequestHandler<GetAllMaintenanceHistoriesQuery, PagedResult<GetAllMaintenanceHistoryDTO>>
{
    private readonly IMaintenanceHistoryRepository _maintenanceHistoryRepository;
    private readonly IWorkOrderLineItemRepository _workOrderLineItemRepository; // Add this
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetAllMaintenanceHistoriesQueryHandler> _logger;
    private readonly IValidator<GetAllMaintenanceHistoriesQuery> _validator;

    public GetAllMaintenanceHistoriesQueryHandler(
        IMaintenanceHistoryRepository maintenanceHistoryRepository,
        IWorkOrderLineItemRepository workOrderLineItemRepository, // Add this
        IMapper mapper,
        IAppLogger<GetAllMaintenanceHistoriesQueryHandler> logger,
        IValidator<GetAllMaintenanceHistoriesQuery> validator)
    {
        _maintenanceHistoryRepository = maintenanceHistoryRepository;
        _workOrderLineItemRepository = workOrderLineItemRepository; // Add this
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<PagedResult<GetAllMaintenanceHistoryDTO>> Handle(GetAllMaintenanceHistoriesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllMaintenanceHistoriesQuery");
        // validate the request
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"GetAllMaintenanceHistories - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // get all maintenance histories from the repository
        var result = await _maintenanceHistoryRepository.GetAllMaintenanceHistoriesPagedAsync(request.Parameters);

        // if no maintenance histories return empty result
        if (!result.Items.Any())
        {
            return new PagedResult<GetAllMaintenanceHistoryDTO>
            {
                Items = [],
                TotalCount = result.TotalCount,
                PageNumber = request.Parameters.PageNumber,
                PageSize = request.Parameters.PageSize
            };
        }

        // Get all work order IDs from maintenance histories
        var workOrderIds = result.Items.Select(mh => mh.WorkOrderID).ToList();

        // Fetch all line items for these work orders in one query
        var allLineItems = await _workOrderLineItemRepository.GetByWorkOrderIdsAsync(workOrderIds);

        // Group line items by WorkOrder ID for efficient lookup
        var lineItemsLookup = allLineItems.GroupBy(li => li.WorkOrderID).ToDictionary(g => g.Key, g => g.ToList());

        // map the maintenance histories to DTOs with cost calculations
        var maintenanceHistoryDTOs = result.Items.Select(mh =>
            CreateMaintenanceHistoryDto(mh, lineItemsLookup)).ToList();

        var pagedResult = new PagedResult<GetAllMaintenanceHistoryDTO>
        {
            Items = maintenanceHistoryDTOs,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        _logger.LogInformation($"Returning {pagedResult.TotalCount} maintenance histories for page {pagedResult.PageNumber} with page size {pagedResult.PageSize}");
        return pagedResult;
    }

    private GetAllMaintenanceHistoryDTO CreateMaintenanceHistoryDto(
        Domain.Entities.MaintenanceHistory maintenanceHistory,
        Dictionary<int, List<Domain.Entities.WorkOrderLineItem>> lineItemsLookup)
    {
        // Use AutoMapper for basic mapping
        var maintenanceHistoryDto = _mapper.Map<GetAllMaintenanceHistoryDTO>(maintenanceHistory);

        // Get line items for this work order (or empty list if none)
        var lineItems = lineItemsLookup.TryGetValue(maintenanceHistory.WorkOrderID, out var items)
            ? items
            : new List<Domain.Entities.WorkOrderLineItem>();

        // Calculate totals using domain services (same as WorkOrder handler)
        if (lineItems.Any())
        {
            lineItems.EnsureTotalCalculated();
            var workOrderLineItemTotals = lineItems.CalculateTotals();

            // Set calculated totals - these properties must exist in GetAllMaintenanceHistoryDTO
            maintenanceHistoryDto.Cost = workOrderLineItemTotals.TotalCost;
        }
        else
        {
            _logger.LogDebug("No line items found for WorkOrder with ID {WorkOrderId} in MaintenanceHistory {MaintenanceHistoryId}",
                maintenanceHistory.WorkOrderID, maintenanceHistory.ID);

            // Set default values when no line items
            maintenanceHistoryDto.Cost = 0;
        }

        return maintenanceHistoryDto;
    }
}