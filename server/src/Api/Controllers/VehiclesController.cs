using Application.Features.Vehicles.Command.CreateVehicle;
using Application.Features.Vehicles.Command.DeactivateVehicle;
using Application.Features.Vehicles.Command.UpdateVehicle;
using Application.Features.Vehicles.Query.GetAllVehicle;
using Application.Features.Vehicles.Query.GetVehicleDetails;
using Application.Models.PaginationModels;

using Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for <see cref="Vehicle"/>
/// </summary>
/// <remarks>
/// <b>API Endpoints</b>:
/// <list type="bullet">
/// <item><b>GET /api/vehicles</b> - <see cref="GetVehicles"/> - <see cref="GetAllVehicleQuery"/></item>
/// <item><b>GET /api/vehicles/{id}</b> - <see cref="GetVehicle"/> - <see cref="GetVehicleDetailsQuery"/></item>
/// <item><b>POST /api/vehicles</b> - <see cref="CreateVehicle"/> - <see cref="CreateVehicleCommand"/></item>
/// <item><b>PUT /api/vehicles/{id}</b> - <see cref="UpdateVehicle"/> - <see cref="UpdateVehicleCommand"/></item>
/// <item><b>PATCH /api/vehicles/{id}/deactivate</b> - <see cref="DeactivateVehicle"/> - <see cref="DeactivateVehicleCommand"/></item>
/// </list>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public sealed class VehiclesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(IMediator mediator, ILogger<VehiclesController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a paginated list of all vehicles.
    /// </summary>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A paginated result containing vehicles.</returns>
    /// <response code="200">Returns the paginated list of vehicles.</response>
    /// <response code="400">Pagination parameters are invalid.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<GetAllVehicleDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<GetAllVehicleDTO>>> GetVehicles(
        [FromQuery] PaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetVehicles)}() - Called");

            var query = new GetAllVehicleQuery(parameters);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Retrieves detailed information about a specific vehicle.
    /// </summary>
    /// <param name="id">The ID of the vehicle to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Detailed vehicle information.</returns>
    /// <response code="200">Returns the vehicle details.</response>
    /// <response code="404">Vehicle not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetVehicleDetailsDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetVehicleDetailsDTO>> GetVehicle(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetVehicle)}() - Called");

            var query = new GetVehicleDetailsQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Creates a new vehicle.
    /// </summary>
    /// <param name="command">The vehicle creation command containing all required vehicle information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the newly created vehicle.</returns>
    /// <response code="201">Vehicle created successfully. Returns the vehicle ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="409">Vehicle with the same VIN or license plate already exists.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> CreateVehicle(
        [FromBody] CreateVehicleCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(CreateVehicle)}() - Called");

            var vehicleId = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetVehicle), new { id = vehicleId }, vehicleId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Updates an existing vehicle's information.
    /// </summary>
    /// <param name="id">The ID of the vehicle to update.</param>
    /// <param name="command">The vehicle update command containing the new vehicle information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the updated vehicle.</returns>
    /// <response code="200">Vehicle updated successfully. Returns the vehicle ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="404">Vehicle not found.</response>
    /// <response code="409">Vehicle with the same VIN or license plate already exists.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<int>> UpdateVehicle(
        int id,
        [FromBody] UpdateVehicleCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(UpdateVehicle)}() - Called");

            if (id != command.VehicleID) return ValidationProblem($"{nameof(UpdateVehicle)} - Route ID and body ID mismatch.");

            var vehicleId = await _mediator.Send(command with { VehicleID = id }, cancellationToken);

            return Ok(vehicleId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Deactivates a vehicle.
    /// </summary>
    /// <param name="id">The ID of the vehicle to deactivate.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the deactivated vehicle.</returns>
    /// <response code="200">Vehicle deactivated successfully. Returns the vehicle ID.</response>
    /// <response code="404">Vehicle not found.</response>
    [HttpPatch("{id:int}/deactivate")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<int>> DeactivateVehicle(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(DeactivateVehicle)}() - Called");

            var command = new DeactivateVehicleCommand(id);
            var vehicleId = await _mediator.Send(command, cancellationToken);

            return Ok(vehicleId);
        }
        catch (Exception)
        {
            throw;
        }
    }
}