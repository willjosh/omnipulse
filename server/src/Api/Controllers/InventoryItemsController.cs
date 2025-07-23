using Application.Features.InventoryItems.Command.CreateInventoryItem;
using Application.Features.InventoryItems.Command.DeleteInventoryItem;
using Application.Features.InventoryItems.Command.UpdateInventoryItem;
using Application.Features.InventoryItems.Query.GetAllInventoryItem;
using Application.Features.InventoryItems.Query.GetInventoryItem;
using Application.Models.PaginationModels;

using Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for <see cref="InventoryItem"/>
/// </summary>
/// <remarks>
/// <b>API Endpoints</b>:
/// <list type="bullet">
/// <item><b>GET /api/inventoryitems</b> - <see cref="GetInventoryItems"/> - <see cref="GetAllInventoryItemQuery"/></item>
/// <item><b>GET /api/inventoryitems/{id}</b> - <see cref="GetInventoryItem"/> - <see cref="GetInventoryItemQuery"/></item>
/// <item><b>POST /api/inventoryitems</b> - <see cref="CreateInventoryItem"/> - <see cref="CreateInventoryItemCommand"/></item>
/// <item><b>PUT /api/inventoryitems/{id}</b> - <see cref="UpdateInventoryItem"/> - <see cref="UpdateInventoryItemCommand"/></item>
/// <item><b>DELETE /api/inventoryitems/{id}</b> - <see cref="DeleteInventoryItem"/> - <see cref="DeleteInventoryItemCommand"/></item>
/// </list>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public sealed class InventoryItemsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<InventoryItemsController> _logger;

    public InventoryItemsController(IMediator mediator, ILogger<InventoryItemsController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a paginated list of all inventory items.
    /// </summary>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A paginated result containing inventory items.</returns>
    /// <response code="200">Returns the paginated list of inventory items.</response>
    /// <response code="400">Pagination parameters are invalid.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<GetAllInventoryItemDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<GetAllInventoryItemDTO>>> GetInventoryItems(
        [FromQuery] PaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetInventoryItems)}() - Called");

            var query = new GetAllInventoryItemQuery(parameters);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetInventoryItems)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving inventory items." });
        }
    }

    /// <summary>
    /// Retrieves detailed information about a specific inventory item.
    /// </summary>
    /// <param name="id">The ID of the inventory item to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Detailed inventory item information.</returns>
    /// <response code="200">Returns the inventory item details.</response>
    /// <response code="404">Inventory item not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetInventoryItemDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetInventoryItemDTO>> GetInventoryItem(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetInventoryItem)}() - Called");

            var query = new GetInventoryItemQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetInventoryItem)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the inventory item." });
        }
    }

    /// <summary>
    /// Creates a new inventory item.
    /// </summary>
    /// <param name="command">The inventory item creation command containing all required inventory item information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the newly created inventory item.</returns>
    /// <response code="201">Inventory item created successfully. Returns the inventory item ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="409">Inventory item with the same item number, UPC, or manufacturer part number already exists.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> CreateInventoryItem(
        [FromBody] CreateInventoryItemCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(CreateInventoryItem)}() - Called");

            var inventoryItemId = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetInventoryItem), new { id = inventoryItemId }, inventoryItemId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(CreateInventoryItem)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the inventory item." });
        }
    }

    /// <summary>
    /// Updates an existing inventory item's information.
    /// </summary>
    /// <param name="id">The ID of the inventory item to update.</param>
    /// <param name="command">The inventory item update command containing the new inventory item information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the updated inventory item.</returns>
    /// <response code="200">Inventory item updated successfully. Returns the inventory item ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="404">Inventory item not found.</response>
    /// <response code="409">Inventory item with the same item number, UPC, or manufacturer part number already exists.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> UpdateInventoryItem(
        int id,
        [FromBody] UpdateInventoryItemCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(UpdateInventoryItem)}() - Called");

            if (id != command.InventoryItemID) return ValidationProblem($"{nameof(UpdateInventoryItem)} - Route ID and body ID mismatch.");

            var inventoryItemId = await _mediator.Send(command with { InventoryItemID = id }, cancellationToken);

            return Ok(inventoryItemId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(UpdateInventoryItem)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the inventory item." });
        }
    }

    /// <summary>
    /// Deletes an inventory item.
    /// </summary>
    /// <param name="id">The ID of the inventory item to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the deleted inventory item.</returns>
    /// <response code="200">Inventory item deleted successfully. Returns the inventory item ID.</response>
    /// <response code="404">Inventory item not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> DeleteInventoryItem(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(DeleteInventoryItem)}() - Called");

            var command = new DeleteInventoryItemCommand(id);
            var inventoryItemId = await _mediator.Send(command, cancellationToken);

            return Ok(inventoryItemId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(DeleteInventoryItem)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the inventory item." });
        }
    }
}