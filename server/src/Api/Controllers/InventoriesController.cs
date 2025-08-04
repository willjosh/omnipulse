using System;

using Application.Contracts.UserServices;
using Application.Features.Inventory.Command.DeleteInventory;
using Application.Features.Inventory.Command.UpdateInventory;
using Application.Features.Inventory.Query;
using Application.Features.Inventory.Query.GetInventory;
using Application.Models.PaginationModels;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for <see cref="Inventory"/>
/// </summary>
/// <remarks>
/// <b>API Endpoints</b>:
/// <list type="bullet">
/// <item><b>GET /api/inventories</b> - <see cref="GetAllInventories"/> - <see cref="GetAllInventoryQuery"/></item>
/// <item><b>GET /api/inventories/{id}</b> - <see cref="GetInventory"/> - <see cref="GetInventoryQuery"/></item>
/// <item><b>PUT /api/inventories/{id}</b> - <see cref="UpdateInventory"/> - <see cref="UpdateInventoryCommand"/></item>
/// <item><b>DELETE /api/inventories/{id}</b> - <see cref="DeleteInventory"/> - <see cref="DeleteInventoryCommand"/></item>
/// </list>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
[Authorize(Policy = "FleetManager")]
public class InventoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<InventoriesController> _logger;
    private readonly ICurrentUserService _currentUserService;

    public InventoriesController(IMediator mediator, ILogger<InventoriesController> logger, ICurrentUserService currentUserService)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    /// <summary>
    /// Retrieves a paginated list of all inventories with optional filtering and sorting.
    /// </summary>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A paginated list of inventories.</returns>
    /// <response code="200">Inventories retrieved successfully.</response>
    /// <response code="400">Request parameters are invalid or validation failed.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<InventoryDetailDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<InventoryDetailDTO>>> GetAllInventories(
        [FromQuery] PaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetAllInventories)}() - Called");

            var query = new GetAllInventoryQuery(parameters);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Retrieves detailed information about a specific inventory by its ID.
    /// </summary>
    /// <param name="id">The ID of the inventory to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The inventory details.</returns>
    /// <response code="200">Inventory retrieved successfully.</response>
    /// <response code="400">Invalid inventory ID provided.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="404">Inventory with the specified ID was not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(InventoryDetailDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InventoryDetailDTO>> GetInventory(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetInventory)}() - Called with ID: {id}");

            var query = new GetInventoryQuery(id);
            var inventoryDto = await _mediator.Send(query, cancellationToken);

            return Ok(inventoryDto);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Updates an existing inventory's information including stock levels and costs.
    /// </summary>
    /// <param name="id">The ID of the inventory to update.</param>
    /// <param name="command">The inventory update command containing the new inventory information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the updated inventory.</returns>
    /// <response code="200">Inventory updated successfully. Returns the inventory ID.</response>
    /// <response code="400">Request data is invalid, validation failed, or route ID and body ID mismatch.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="404">Inventory not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> UpdateInventory(
        int id,
        [FromBody] UpdateInventoryCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(UpdateInventory)}() - Called with ID: {id}");

            // Validate that route ID matches command ID
            if (id != command.InventoryID)
            {
                return ValidationProblem($"{nameof(UpdateInventory)} - Route ID ({id}) and body ID ({command.InventoryID}) mismatch.");
            }

            var userID = _currentUserService.UserId;

            // Ensure the command uses the route ID (security measure)
            var inventoryId = await _mediator.Send(command with { InventoryID = id, PerformedByUserID = userID! }, cancellationToken);

            return Ok(inventoryId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Deletes an inventory.
    /// </summary>
    /// <param name="id">The ID of the inventory to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the deleted inventory.</returns>
    /// <response code="200">Inventory deleted successfully. Returns the inventory ID.</response>
    /// <response code="404">Inventory not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> DeleteInventory(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(DeleteInventory)}() - Called");

            var command = new DeleteInventoryCommand(id);
            var inventoryId = await _mediator.Send(command, cancellationToken);

            return Ok(inventoryId);
        }
        catch (Exception)
        {
            throw;
        }
    }
}