using Application.Features.FuelPurchases.Command.CreateFuelPurchase;
using Application.Features.FuelPurchases.Command.DeleteFuelPurchase;
using Application.Features.FuelPurchases.Command.UpdateFuelPurchase;
using Application.Features.FuelPurchases.Query;
using Application.Features.FuelPurchases.Query.GetAllFuelPurchase;
using Application.Features.FuelPurchases.Query.GetFuelPurchase;
using Application.Models.PaginationModels;

using Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for <see cref="FuelPurchase"/>
/// </summary>
/// <remarks>
/// <b>API Endpoints</b>:
/// <list type="bullet">
/// <item><b>GET /api/fuelpurchases</b> - <see cref="GetAllFuelPurchases"/> - <see cref="GetAllFuelPurchaseQuery"/></item>
/// <item><b>GET /api/fuelpurchases/{id:int}</b> - <see cref="GetFuelPurchase"/> - <see cref="GetFuelPurchaseQuery"/></item>
/// <item><b>POST /api/fuelpurchases</b> - <see cref="CreateFuelPurchase"/> - <see cref="CreateFuelPurchaseCommand"/></item>
/// <item><b>PUT /api/fuelpurchases/{id:int}</b> - <see cref="UpdateFuelPurchase"/> - <see cref="UpdateFuelPurchaseCommand"/></item>
/// <item><b>DELETE /api/fuelpurchases/{id:int}</b> - <see cref="DeleteFuelPurchase"/> - <see cref="DeleteFuelPurchaseCommand"/></item>
/// </list>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public sealed class FuelPurchasesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FuelPurchasesController> _logger;

    public FuelPurchasesController(IMediator mediator, ILogger<FuelPurchasesController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a paginated list of all fuel purchases.
    /// </summary>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A paginated list of fuel purchases.</returns>
    /// <response code="200">Fuel purchases retrieved successfully.</response>
    /// <response code="400">Request parameters are invalid or validation failed.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<FuelPurchaseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<FuelPurchaseDTO>>> GetAllFuelPurchases(
        [FromQuery] PaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetAllFuelPurchases)}() - Called");

            var query = new GetAllFuelPurchaseQuery(parameters);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Gets a fuel purchase by ID.
    /// </summary>
    /// <param name="id">The ID of the fuel purchase.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The fuel purchase DTO.</returns>
    /// <response code="200">Returns the fuel purchase.</response>
    /// <response code="400">Invalid ID.</response>
    /// <response code="404">Not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FuelPurchaseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FuelPurchaseDTO>> GetFuelPurchase(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetFuelPurchase)}() - Called");

            var query = new GetFuelPurchaseQuery(id);
            var dto = await _mediator.Send(query, cancellationToken);

            return Ok(dto);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Creates a new fuel purchase entry.
    /// </summary>
    /// <param name="command">The fuel purchase creation command containing all required fuel purchase information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the newly created fuel purchase.</returns>
    /// <response code="201">Fuel purchase created successfully. Returns the fuel purchase ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="404">Vehicle or user not found.</response>
    /// <response code="409">Receipt number already exists.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> CreateFuelPurchase(
        [FromBody] CreateFuelPurchaseCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(CreateFuelPurchase)}() - Called");

            var fuelPurchaseId = await _mediator.Send(command, cancellationToken);

            // TODO: Change Location header from nameof(CreateFuelPurchase) to nameof(GetFuelPurchase)
            return CreatedAtAction(nameof(CreateFuelPurchase), new { id = fuelPurchaseId }, fuelPurchaseId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Updates an existing fuel purchase entry.
    /// </summary>
    /// <param name="id">The ID of the fuel purchase to update.</param>
    /// <param name="command">The update command containing the new fuel purchase information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the updated fuel purchase.</returns>
    /// <response code="200">Fuel purchase updated successfully. Returns the fuel purchase ID.</response>
    /// <response code="400">Request data is invalid, validation failed, or route ID and body ID mismatch.</response>
    /// <response code="404">Fuel purchase, vehicle, or user not found.</response>
    /// <response code="409">Receipt number already exists.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> UpdateFuelPurchase(
        int id,
        [FromBody] UpdateFuelPurchaseCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(UpdateFuelPurchase)}() - Called");

            if (id != command.FuelPurchaseId) return BadRequest($"{nameof(UpdateFuelPurchase)} - Route ID and body ID mismatch.");
            var fuelPurchaseId = await _mediator.Send(command with { FuelPurchaseId = id }, cancellationToken);

            return Ok(fuelPurchaseId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Deletes a fuel purchase.
    /// </summary>
    /// <param name="id">The ID of the fuel purchase to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the deleted fuel purchase.</returns>
    /// <response code="200">Fuel purchase deleted successfully. Returns the fuel purchase ID.</response>
    /// <response code="404">Fuel purchase not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> DeleteFuelPurchase(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(DeleteFuelPurchase)}() - Called");

            var command = new DeleteFuelPurchaseCommand(id);
            var fuelPurchaseId = await _mediator.Send(command, cancellationToken);

            return Ok(fuelPurchaseId);
        }
        catch (Exception)
        {
            throw;
        }
    }
}