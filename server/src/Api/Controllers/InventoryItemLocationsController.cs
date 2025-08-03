using Application.Features.InventoryItemLocations.Command;
using Application.Features.InventoryItemLocations.Command.DeleteInventoryItemLocation;
using Application.Features.InventoryItemLocations.Query.GetAllInventoryItemLocation;

using Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for <see cref="InventoryItemLocation"/>
/// </summary>
/// <remarks>
/// <b>API Endpoints</b>:
/// <list type="bullet">
/// <item><b>GET /api/inventoryitemlocations</b> - <see cref="GetInventoryItemLocations"/> - <see cref="GetAllInventoryItemLocationQuery"/></item>
/// <item><b>POST /api/inventoryitemlocations</b> - <see cref="CreateInventoryItemLocation"/> - <see cref="CreateInventoryItemLocationCommand"/></item>
/// <item><b>DELETE /api/inventoryitemlocations/{id}</b> - <see cref="DeleteInventoryItemLocation"/> - <see cref="DeleteInventoryItemLocationCommand"/></item>
/// </list>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
[Authorize(Policy = "FleetManager")]
public sealed class InventoryItemLocationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<InventoryItemLocationsController> _logger;

    public InventoryItemLocationsController(IMediator mediator, ILogger<InventoryItemLocationsController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a list of all inventory item locations.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A list containing all inventory item locations.</returns>
    /// <response code="200">Returns the list of inventory item locations.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<GetAllInventoryItemLocationDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllInventoryItemLocationDTO>>> GetInventoryItemLocations(
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetInventoryItemLocations)}() - Called");

            var query = new GetAllInventoryItemLocationQuery();
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Creates a new inventory item location.
    /// </summary>
    /// <param name="command">The inventory item location creation command containing all required inventory item location information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the newly created inventory item location.</returns>
    /// <response code="201">Inventory item location created successfully. Returns the inventory item location ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> CreateInventoryItemLocation(
        [FromBody] CreateInventoryItemLocationCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(CreateInventoryItemLocation)}() - Called");

            var inventoryItemLocationId = await _mediator.Send(command, cancellationToken);

            // TODO: Change Location header from nameof(GetInventoryItemLocations) to nameof(GetInventoryItemLocation)
            return CreatedAtAction(nameof(GetInventoryItemLocations), new { id = inventoryItemLocationId }, inventoryItemLocationId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Deletes an inventory item location.
    /// </summary>
    /// <param name="id">The ID of the inventory item location to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the deleted inventory item location.</returns>
    /// <response code="200">Inventory item location deleted successfully. Returns the inventory item location ID.</response>
    /// <response code="404">Inventory item location not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> DeleteInventoryItemLocation(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(DeleteInventoryItemLocation)}() - Called");

            var command = new DeleteInventoryItemLocationCommand(id);
            var inventoryItemLocationId = await _mediator.Send(command, cancellationToken);

            return Ok(inventoryItemLocationId);
        }
        catch (Exception)
        {
            throw;
        }
    }
}