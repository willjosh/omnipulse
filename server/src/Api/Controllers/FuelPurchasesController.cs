using Application.Features.FuelLogging.Command.CreateFuelPurchase;
using Application.Features.FuelLogging.Command.UpdateFuelPurchase;

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
/// <item><b>POST /api/fuelpurchases</b> - <see cref="CreateFuelPurchase"/> - <see cref="CreateFuelPurchaseCommand"/></item>
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
}