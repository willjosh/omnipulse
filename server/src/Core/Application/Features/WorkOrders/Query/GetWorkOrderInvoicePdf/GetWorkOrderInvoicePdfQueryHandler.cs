using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Contracts.Services;
using Application.Exceptions;

using Domain.Entities;
using Domain.Services;

using MediatR;

namespace Application.Features.WorkOrders.Query.GetWorkOrderInvoicePdf;

public class GetWorkOrderInvoicePdfQueryHandler : IRequestHandler<GetWorkOrderInvoicePdfQuery, byte[]>
{
    private readonly IInvoicePdfService _invoicePdfService;
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IWorkOrderLineItemRepository _workOrderLineItemRepository;
    private readonly IAppLogger<GetWorkOrderInvoicePdfQueryHandler> _logger;

    public GetWorkOrderInvoicePdfQueryHandler(
        IInvoicePdfService invoicePdfService,
        IWorkOrderRepository workOrderRepository,
        IWorkOrderLineItemRepository workOrderLineItemRepository,
        IAppLogger<GetWorkOrderInvoicePdfQueryHandler> logger)
    {
        _invoicePdfService = invoicePdfService ?? throw new ArgumentNullException(nameof(invoicePdfService));
        _workOrderRepository = workOrderRepository ?? throw new ArgumentNullException(nameof(workOrderRepository));
        _workOrderLineItemRepository = workOrderLineItemRepository ?? throw new ArgumentNullException(nameof(workOrderLineItemRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<byte[]> Handle(GetWorkOrderInvoicePdfQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)}() - Handling {nameof(GetWorkOrderInvoicePdfQuery)}({nameof(GetWorkOrderInvoicePdfQuery.WorkOrderId)}: {request.WorkOrderId})");

        // Get work order with related data
        var workOrder = await _workOrderRepository.GetWorkOrderWithDetailsAsync(request.WorkOrderId);
        if (workOrder == null)
        {
            _logger.LogError("WorkOrder with ID {WorkOrderId} not found", request.WorkOrderId);
            throw new EntityNotFoundException(nameof(WorkOrder), nameof(GetWorkOrderInvoicePdfQuery.WorkOrderId), request.WorkOrderId.ToString());
        }

        var workOrderLineItems = await _workOrderLineItemRepository.GetByWorkOrderIdAsync(request.WorkOrderId);
        var lineItems = workOrderLineItems ?? [];

        // Ensure line items have calculated totals
        lineItems.EnsureTotalCalculated();

        // Calculate totals using domain extensions
        var totals = lineItems.CalculateTotals();

        // Create invoice data for PDF generation
        var invoiceData = new GetWorkOrderInvoicePdfDTO
        {
            WorkOrderID = workOrder.ID,
            WorkOrderTitle = workOrder.Title,
            WorkOrderDescription = workOrder.Description,
            WorkOrderType = workOrder.WorkOrderType.ToString(),
            WorkOrderPriorityLevel = workOrder.PriorityLevel.ToString(),
            ScheduledStartDate = workOrder.ScheduledStartDate,
            ActualStartDate = workOrder.ActualStartDate,
            VehicleName = workOrder.Vehicle.Name,
            VehicleMake = workOrder.Vehicle.Make,
            VehicleModel = workOrder.Vehicle.Model,
            VehicleVIN = workOrder.Vehicle.VIN,
            VehicleLicensePlate = workOrder.Vehicle.LicensePlate,
            AssignedToUserName = workOrder.User.GetFullName(),
            WorkOrderLineItems = MapLineItemsToDto(lineItems),
            TotalLabourCost = totals.TotalLaborCost,
            TotalItemCost = totals.TotalItemCost,
            TotalCost = totals.TotalCost,
            InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{workOrder.ID}",
            InvoiceDate = DateTime.UtcNow
        };

        // Generate PDF directly
        var pdfBytes = await _invoicePdfService.GenerateInvoicePdfAsync(invoiceData);

        return pdfBytes;
    }

    private static List<WorkOrderLineItemInvoicePdfDTO> MapLineItemsToDto(List<Domain.Entities.WorkOrderLineItem> lineItems)
    {
        return lineItems.Select(item => new WorkOrderLineItemInvoicePdfDTO
        {
            ID = item.ID,
            WorkOrderID = item.WorkOrderID,
            ServiceTaskID = item.ServiceTaskID,
            ServiceTaskName = item.ServiceTask?.Name ?? "Unknown Task",
            ItemType = item.ItemType.ToString(),
            Quantity = item.Quantity,
            TotalCost = item.TotalCost,
            InventoryItemID = item.InventoryItemID,
            Description = item.Description,
            LaborHours = item.LaborHours.HasValue ? (decimal)item.LaborHours.Value : null,
            UnitPrice = item.UnitPrice,
            HourlyRate = item.HourlyRate
        }).ToList();
    }
}