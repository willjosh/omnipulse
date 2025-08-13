using Application.Features.VehicleGroups.Command.CreateVehicleGroup;
using Application.Features.VehicleGroups.Command.DeleteVehicleGroup;
using Application.Features.VehicleGroups.Command.UpdateVehicleGroup;
using Application.Features.VehicleGroups.Query.GetAllVehicleGroup;
using Application.Models.PaginationModels;

using Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for <see cref="VehicleGroup"/>
/// </summary>
/// <remarks>
/// <b>API Endpoints</b>:
/// <list type="bullet">
/// <item><b>GET /api/vehiclegroups</b> - <see cref="GetVehicleGroups"/> - <see cref="GetAllVehicleGroupQuery"/></item>
/// <item><b>POST /api/vehiclegroups</b> - <see cref="CreateVehicleGroup"/> - <see cref="CreateVehicleGroupCommand"/></item>
/// <item><b>PUT /api/vehiclegroups/{id}</b> - <see cref="UpdateVehicleGroup"/> - <see cref="UpdateVehicleGroupCommand"/></item>
/// <item><b>DELETE /api/vehiclegroups/{id}</b> - <see cref="DeleteVehicleGroup"/> - <see cref="DeleteVehicleGroupCommand"/></item>
/// </list>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
[Authorize(Policy = "FleetManager")]
public sealed class VehicleGroupsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<VehicleGroupsController> _logger;

    public VehicleGroupsController(IMediator mediator, ILogger<VehicleGroupsController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a paginated list of all vehicle groups.
    /// </summary>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A paginated result containing vehicle groups.</returns>
    /// <response code="200">Returns the paginated list of vehicle groups.</response>
    /// <response code="400">Pagination parameters are invalid.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<GetAllVehicleGroupDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<GetAllVehicleGroupDTO>>> GetVehicleGroups(
        [FromQuery] PaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetVehicleGroups)}() - Called");

            var query = new GetAllVehicleGroupQuery(parameters);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Creates a new vehicle group.
    /// </summary>
    /// <param name="command">The vehicle group creation command containing all required vehicle group information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the newly created vehicle group.</returns>
    /// <response code="201">Vehicle group created successfully. Returns the vehicle group ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> CreateVehicleGroup(
        [FromBody] CreateVehicleGroupCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(CreateVehicleGroup)}() - Called");

            var vehicleGroupId = await _mediator.Send(command, cancellationToken);

            // Change Location header from nameof(GetVehicleGroups) to nameof(GetVehicleGroup) if GetVehicleGroup is implemented
            return CreatedAtAction(nameof(GetVehicleGroups), new { id = vehicleGroupId }, vehicleGroupId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Updates an existing vehicle group's information.
    /// </summary>
    /// <param name="id">The ID of the vehicle group to update.</param>
    /// <param name="command">The vehicle group update command containing the new vehicle group information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the updated vehicle group.</returns>
    /// <response code="200">Vehicle group updated successfully. Returns the vehicle group ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="404">Vehicle group not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> UpdateVehicleGroup(
        int id,
        [FromBody] UpdateVehicleGroupCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(UpdateVehicleGroup)}() - Called");

            if (id != command.VehicleGroupId) return ValidationProblem($"{nameof(UpdateVehicleGroup)} - Route ID and body ID mismatch.");

            var vehicleGroupId = await _mediator.Send(command with { VehicleGroupId = id }, cancellationToken);

            return Ok(vehicleGroupId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Deletes a vehicle group.
    /// </summary>
    /// <param name="id">The ID of the vehicle group to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the deleted vehicle group.</returns>
    /// <response code="200">Vehicle group deleted successfully. Returns the vehicle group ID.</response>
    /// <response code="404">Vehicle group not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> DeleteVehicleGroup(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(DeleteVehicleGroup)}() - Called");

            var command = new DeleteVehicleGroupCommand(id);
            var vehicleGroupId = await _mediator.Send(command, cancellationToken);

            return Ok(vehicleGroupId);
        }
        catch (Exception)
        {
            throw;
        }
    }
}