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
    private readonly IUserRepository _userRepository;
    private readonly IWorkOrderLineItemRepository _workOrderLineItemRepository;
    private readonly IAppLogger<GetWorkOrderInvoicePdfQueryHandler> _logger;

    public GetWorkOrderInvoicePdfQueryHandler(
        IInvoicePdfService invoicePdfService,
        IWorkOrderRepository workOrderRepository,
        IUserRepository userRepository,
        IWorkOrderLineItemRepository workOrderLineItemRepository,
        IAppLogger<GetWorkOrderInvoicePdfQueryHandler> logger)
    {
        _invoicePdfService = invoicePdfService ?? throw new ArgumentNullException(nameof(invoicePdfService));
        _workOrderRepository = workOrderRepository ?? throw new ArgumentNullException(nameof(workOrderRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _workOrderLineItemRepository = workOrderLineItemRepository ?? throw new ArgumentNullException(nameof(workOrderLineItemRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<byte[]> Handle(GetWorkOrderInvoicePdfQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)}() - Handling {nameof(GetWorkOrderInvoicePdfQuery)}({nameof(GetWorkOrderInvoicePdfQuery.WorkOrderId)}: {request.WorkOrderId})");

        // get the user who issued the work order
        var issuedByUser = await _userRepository.GetByIdAsync(request.IssuedByUserID);
        if (issuedByUser == null)
        {
            _logger.LogError("IssuedBy user for WorkOrder with ID {WorkOrderId} not found", request.WorkOrderId);
            throw new EntityNotFoundException(nameof(User), nameof(GetWorkOrderInvoicePdfQuery.IssuedByUserID), request.IssuedByUserID);
        }

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
            WorkOrderStatus = workOrder.Status.ToString(),
            ExpectedCompletionDate = workOrder.ScheduledCompletionDate,
            ActualCompletionDate = workOrder.ActualCompletionDate,
            ScheduledStartDate = workOrder.ScheduledStartDate,
            ActualStartDate = workOrder.ActualStartDate,
            VehicleName = workOrder.Vehicle.Name,
            VehicleMake = workOrder.Vehicle.Make,
            VehicleModel = workOrder.Vehicle.Model,
            VehicleVIN = workOrder.Vehicle.VIN,
            VehicleType = workOrder.Vehicle.VehicleType.ToString(),
            VehicleGroupName = workOrder.Vehicle.VehicleGroup?.Name,
            VehicleOdometer = workOrder.Vehicle.Mileage,
            VehicleLicensePlate = workOrder.Vehicle.LicensePlate,
            IssuedByUserName = issuedByUser.GetFullName(),
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
        // TODO add missing fields
        return lineItems.Select(item =>
        {
            item.CalculateTotalCost(); // Ensure each item has its total calculated
            return new WorkOrderLineItemInvoicePdfDTO
            {
                InventoryItemName = item.InventoryItem?.ItemName,
                ServiceTaskName = item.ServiceTask?.Name ?? "Unknown Task",
                LaborTotal = item.CalculateLaborCost(),
                ItemTotal = item.CalculateItemCost(),
                SubTotal = item.TotalCost
            };
        }).ToList();
    }
}