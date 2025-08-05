using Application.Contracts.Services;
using Application.Contracts.UserServices;
using Application.Features.WorkOrders.Command.CompleteWorkOrder;
using Application.Features.WorkOrders.Command.CreateWorkOrder;
using Application.Features.WorkOrders.Command.DeleteWorkOrder;
using Application.Features.WorkOrders.Command.UpdateWorkOrder;
using Application.Features.WorkOrders.Query.GetAllWorkOrder;
using Application.Features.WorkOrders.Query.GetWorkOrderDetail;
using Application.Features.WorkOrders.Query.GetWorkOrderInvoicePdf;
using Application.Features.WorkOrders.Query.GetWorkOrderStatusData; // ✅ Add this using
using Application.Models.PaginationModels;

using Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for <see cref="WorkOrder"/>
/// </summary>
/// <remarks>
/// <b>API Endpoints</b>:
/// <list type="bullet">
/// <item><b>GET /api/workorders</b> - <see cref="GetWorkOrders"/> - <see cref="GetAllWorkOrderQuery"/></item>
/// <item><b>GET /api/workorders/{id}</b> - <see cref="GetWorkOrder"/> - <see cref="GetWorkOrderDetailQuery"/></item>
/// <item><b>GET /api/workorders/status-data</b> - <see cref="GetWorkOrderStatusData"/> - <see cref="GetWorkOrderStatusDataQuery"/></item>
/// <item><b>POST /api/workorders</b> - <see cref="CreateWorkOrder"/> - <see cref="CreateWorkOrderCommand"/></item>
/// <item><b>PUT /api/workorders/{id}</b> - <see cref="UpdateWorkOrder"/> - <see cref="UpdateWorkOrderCommand"/></item>
/// <item><b>DELETE /api/workorders/{id}</b> - <see cref="DeleteWorkOrder"/> - <see cref="DeleteWorkOrderCommand"/></item>
/// <item><b>PATCH /api/workorders/{id}/complete</b> - <see cref="CompleteWorkOrder"/> - <see cref="CompleteWorkOrderCommand"/></item>
/// <item><b>GET /api/workorders/{id}/invoice.pdf</b> - <see cref="GetWorkOrderInvoicePdf"/> - <see cref="GetWorkOrderInvoicePdfQuery"/></item>
/// </list>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
[Authorize]
public sealed class WorkOrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<WorkOrdersController> _logger;
    private readonly ICurrentUserService _currentUserService;

    public WorkOrdersController(IMediator mediator, ILogger<WorkOrdersController> logger, ICurrentUserService currentUserService)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    /// <summary>
    /// Retrieves a paginated list of all work orders.
    /// </summary>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A paginated result containing work orders.</returns>
    /// <response code="200">Returns the paginated list of work orders.</response>
    /// <response code="400">Pagination parameters are invalid.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<GetWorkOrderDetailDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "AllRoles")]
    public async Task<ActionResult<PagedResult<GetWorkOrderDetailDTO>>> GetWorkOrders(
        [FromQuery] PaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetWorkOrders)}() - Called");

            var query = new GetAllWorkOrderQuery(parameters);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Retrieves detailed information about a specific work order.
    /// </summary>
    /// <param name="id">The ID of the work order to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Detailed work order information.</returns>
    /// <response code="200">Returns the work order details.</response>
    /// <response code="404">Work order not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetWorkOrderDetailDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "AllRoles")]
    public async Task<ActionResult<GetWorkOrderDetailDTO>> GetWorkOrder(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetWorkOrder)}() - Called");

            var query = new GetWorkOrderDetailQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Creates a new work order.
    /// </summary>
    /// <param name="command">The work order creation command containing all required work order information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the newly created work order.</returns>
    /// <response code="201">Work order created successfully. Returns the work order ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="404">Vehicle, user, issue, inventory item, or service task not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "FleetManager")]
    public async Task<ActionResult<int>> CreateWorkOrder(
        [FromBody] CreateWorkOrderCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(CreateWorkOrder)}() - Called");

            var workOrderId = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetWorkOrder), new { id = workOrderId }, workOrderId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Updates an existing work order.
    /// </summary>
    /// <param name="id">The ID of the work order to update.</param>
    /// <param name="command">The work order update command containing the new work order information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the updated work order.</returns>
    /// <response code="200">Work order updated successfully. Returns the work order ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="404">Work order, vehicle, user, issue, inventory item, or service task not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "AllRoles")]
    public async Task<ActionResult<int>> UpdateWorkOrder(
        int id,
        [FromBody] UpdateWorkOrderCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(UpdateWorkOrder)}() - Called");

            if (id != command.WorkOrderID)
                return ValidationProblem($"{nameof(UpdateWorkOrder)} - Route ID and body ID mismatch.");

            var workOrderId = await _mediator.Send(command with { WorkOrderID = id }, cancellationToken);

            return Ok(workOrderId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Deletes a work order.
    /// </summary>
    /// <param name="id">The ID of the work order to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the deleted work order.</returns>
    /// <response code="200">Work order deleted successfully. Returns the work order ID.</response>
    /// <response code="404">Work order not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "FleetManager")]
    public async Task<ActionResult<int>> DeleteWorkOrder(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(DeleteWorkOrder)}() - Called");

            var command = new DeleteWorkOrderCommand(id);
            var workOrderId = await _mediator.Send(command, cancellationToken);

            return Ok(workOrderId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Completes a work order by updating its status to completed and processing inventory transactions.
    /// </summary>
    /// <param name="id">The ID of the work order to complete.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the completed work order.</returns>
    /// <response code="200">Work order completed successfully. Returns the work order ID.</response>
    /// <response code="400">Work order is not ready for completion or validation failed.</response>
    /// <response code="404">Work order not found.</response>
    /// <response code="409">Work order is already completed.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPatch("{id:int}/complete")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "FleetManager")]
    public async Task<ActionResult<int>> CompleteWorkOrder(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(CompleteWorkOrder)}() - Called for WorkOrder ID: {id}");

            var command = new CompleteWorkOrderCommand(id);
            var workOrderId = await _mediator.Send(command, cancellationToken);

            return Ok(workOrderId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Generates a PDF invoice for a specific work order.
    /// </summary>
    /// <param name="id">The ID of the work order to generate an invoice for.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>PDF invoice as a file download.</returns>
    /// <response code="200">Returns the PDF invoice.</response>
    /// <response code="404">Work order not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet("{id:int}/invoice.pdf")]
    [Produces("application/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "AllRoles")]
    public async Task<IActionResult> GetWorkOrderInvoicePdf(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetWorkOrderInvoicePdf)}() - Called");

            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID is required to generate invoice.");

            // Get PDF bytes directly from query
            var query = new GetWorkOrderInvoicePdfQuery(id, userId);
            var pdfBytes = await _mediator.Send(query, cancellationToken);

            // Return PDF as file download
            var fileName = $"workorder{id}-invoice.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Retrieves work order status statistics (created and in-progress counts).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Work order status data including counts of created and in-progress work orders.</returns>
    /// <response code="200">Returns the work order status statistics.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet("status-data")]
    [ProducesResponseType(typeof(GetWorkOrderStatusDataDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "AllRoles")]
    public async Task<ActionResult<GetWorkOrderStatusDataDTO>> GetWorkOrderStatusData(
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetWorkOrderStatusData)}() - Called");

            var query = new GetWorkOrderStatusDataQuery();
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception)
        {
            throw;
        }
    }
}