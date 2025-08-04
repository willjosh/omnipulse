using System;

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

    public InventoriesController(IMediator mediator, ILogger<InventoriesController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
}