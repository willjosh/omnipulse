using Application.Features.WorkOrders.Command.CreateWorkOrder;
using Application.Features.WorkOrders.Query.GetWorkOrderDetail;

using Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for <see cref="WorkOrder"/>
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public sealed class WorkOrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<WorkOrdersController> _logger;

    public WorkOrdersController(IMediator mediator, ILogger<WorkOrdersController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetWorkOrder)} - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the work order." });
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
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(CreateWorkOrder)} - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the work order." });
        }
    }
}