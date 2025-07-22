using Application.Features.ServicePrograms.Command.AddVehicleToServiceProgram;
using Application.Features.ServicePrograms.Command.CreateServiceProgram;
using Application.Features.ServicePrograms.Command.DeleteServiceProgram;
using Application.Features.ServicePrograms.Command.RemoveVehicleFromServiceProgram;
using Application.Features.ServicePrograms.Command.UpdateServiceProgram;
using Application.Features.ServicePrograms.Query;
using Application.Features.ServicePrograms.Query.GetAllServiceProgram;
using Application.Features.ServicePrograms.Query.GetAllServiceProgramVehicle;
using Application.Features.ServicePrograms.Query.GetServiceProgram;
using Application.Models.PaginationModels;

using Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for <see cref="ServiceProgram"/>
/// </summary>
/// <remarks>
/// <b>API Endpoints</b>:
/// <list type="bullet">
/// <item><b>GET /api/serviceprograms</b> - <see cref="GetServicePrograms"/> - <see cref="GetAllServiceProgramQuery"/></item>
/// <item><b>GET /api/serviceprograms/{id}</b> - <see cref="GetServiceProgram"/> - <see cref="GetServiceProgramQuery"/></item>
/// <item><b>POST /api/serviceprograms</b> - <see cref="CreateServiceProgram"/> - <see cref="CreateServiceProgramCommand"/></item>
/// <item><b>PUT /api/serviceprograms/{id}</b> - <see cref="UpdateServiceProgram"/> - <see cref="UpdateServiceProgramCommand"/></item>
/// <item><b>DELETE /api/serviceprograms/{id}</b> - <see cref="DeleteServiceProgram"/> - <see cref="DeleteServiceProgramCommand"/></item>
/// <item><b>GET /api/serviceprograms/{serviceProgramId}/vehicles</b> - <see cref="GetServiceProgramVehicles"/> - <see cref="GetAllServiceProgramVehicleQuery"/></item>
/// <item><b>POST /api/serviceprograms/{id}/vehicles</b> - <see cref="AddVehicleToServiceProgram"/> - <see cref="AddVehicleToServiceProgramCommand"/></item>
/// <item><b>DELETE /api/serviceprograms/{id}/vehicles/{vehicleId}</b> - <see cref="RemoveVehicleFromServiceProgram"/> - <see cref="RemoveVehicleFromServiceProgramCommand"/></item>
/// </list>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public sealed class ServiceProgramsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ServiceProgramsController> _logger;

    public ServiceProgramsController(IMediator mediator, ILogger<ServiceProgramsController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a paginated list of all service programs.
    /// </summary>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A paginated result containing service programs.</returns>
    /// <response code="200">Returns the paginated list of service programs.</response>
    /// <response code="400">Pagination parameters are invalid.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<GetAllServiceProgramDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<GetAllServiceProgramDTO>>> GetServicePrograms(
        [FromQuery] PaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetServicePrograms)}() - Called");

            var query = new GetAllServiceProgramQuery(parameters);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetServicePrograms)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving service programs." });
        }
    }

    /// <summary>
    /// Retrieves detailed information about a specific service program.
    /// </summary>
    /// <param name="id">The ID of the service program to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Detailed service program information.</returns>
    /// <response code="200">Returns the service program details.</response>
    /// <response code="404">Service program not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ServiceProgramDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServiceProgramDTO>> GetServiceProgram(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetServiceProgram)}() - Called");

            var query = new GetServiceProgramQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetServiceProgram)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the service program." });
        }
    }

    /// <summary>
    /// Creates a new service program.
    /// </summary>
    /// <param name="command">The service program creation command containing all required service program information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the newly created service program.</returns>
    /// <response code="201">Service program created successfully. Returns the service program ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="409">Service program with the same name already exists.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> CreateServiceProgram(
        [FromBody] CreateServiceProgramCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(CreateServiceProgram)}() - Called");

            var serviceProgramId = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetServiceProgram), new { id = serviceProgramId }, serviceProgramId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(CreateServiceProgram)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the service program." });
        }
    }

    /// <summary>
    /// Updates an existing service program's information.
    /// </summary>
    /// <param name="id">The ID of the service program to update.</param>
    /// <param name="command">The service program update command containing the new service program information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the updated service program.</returns>
    /// <response code="200">Service program updated successfully. Returns the service program ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="404">Service program not found.</response>
    /// <response code="409">Service program with the same name already exists.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> UpdateServiceProgram(
        int id,
        [FromBody] UpdateServiceProgramCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(UpdateServiceProgram)}() - Called");

            if (id != command.ServiceProgramID) return ValidationProblem($"{nameof(UpdateServiceProgram)} - Route ID and body ID mismatch.");

            var serviceProgramId = await _mediator.Send(command with { ServiceProgramID = id }, cancellationToken);

            return Ok(serviceProgramId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(UpdateServiceProgram)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the service program." });
        }
    }

    /// <summary>
    /// Deletes a service program.
    /// </summary>
    /// <param name="id">The ID of the service program to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the deleted service program.</returns>
    /// <response code="200">Service program deleted successfully. Returns the service program ID.</response>
    /// <response code="404">Service program not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> DeleteServiceProgram(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(DeleteServiceProgram)}() - Called");

            var command = new DeleteServiceProgramCommand(id);
            var serviceProgramId = await _mediator.Send(command, cancellationToken);

            return Ok(serviceProgramId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(DeleteServiceProgram)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the service program." });
        }
    }

    /// <summary>
    /// Retrieves a paginated list of all vehicles assigned to a specific service program.
    /// </summary>
    /// <param name="serviceProgramId">The ID of the service program.</param>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A paginated result containing vehicles assigned to the service program.</returns>
    /// <response code="200">Returns the paginated list of assigned vehicles.</response>
    /// <response code="400">Pagination parameters are invalid.</response>
    /// <response code="404">Service program not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet("{serviceProgramId:int}/vehicles")]
    [ProducesResponseType(typeof(PagedResult<XrefServiceProgramVehicleDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<XrefServiceProgramVehicleDTO>>> GetServiceProgramVehicles(
        int serviceProgramId,
        [FromQuery] PaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetServiceProgramVehicles)}() - Called");

            var query = new GetAllServiceProgramVehicleQuery(serviceProgramId, parameters);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetServiceProgramVehicles)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving assigned vehicles for the service program." });
        }
    }

    /// <summary>
    /// Adds a vehicle to a service program.
    /// </summary>
    /// <param name="serviceProgramId">The ID of the service program.</param>
    /// <param name="command">The command containing the vehicle ID to add to the service program.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A tuple containing the service program ID and vehicle ID.</returns>
    /// <response code="200">Vehicle added to service program successfully. Returns the service program ID and vehicle ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="404">Service program or vehicle not found.</response>
    /// <response code="409">Vehicle is already assigned to this service program.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost("{serviceProgramId:int}/vehicles")]
    [ProducesResponseType(typeof((int ServiceProgramID, int VehicleID)), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<(int ServiceProgramID, int VehicleID)>> AddVehicleToServiceProgram(
        int serviceProgramId,
        [FromBody] AddVehicleToServiceProgramCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(AddVehicleToServiceProgram)}() - Called");

            if (serviceProgramId != command.ServiceProgramID) return ValidationProblem($"{nameof(AddVehicleToServiceProgram)} - Route ID and body ID mismatch.");

            var result = await _mediator.Send(command with { ServiceProgramID = serviceProgramId }, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(AddVehicleToServiceProgram)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while adding the vehicle to the service program." });
        }
    }

    /// <summary>
    /// Removes a vehicle from a service program.
    /// </summary>
    /// <param name="serviceProgramId">The ID of the service program.</param>
    /// <param name="vehicleId">The ID of the vehicle to remove from the service program.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A tuple containing the service program ID and vehicle ID.</returns>
    /// <response code="200">Vehicle removed from service program successfully. Returns the service program ID and vehicle ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="404">Service program, vehicle, or assignment not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpDelete("{serviceProgramId:int}/vehicles/{vehicleId:int}")]
    [ProducesResponseType(typeof((int ServiceProgramID, int VehicleID)), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<(int ServiceProgramID, int VehicleID)>> RemoveVehicleFromServiceProgram(
        int serviceProgramId,
        int vehicleId,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(RemoveVehicleFromServiceProgram)}() - Called");

            var command = new RemoveVehicleFromServiceProgramCommand(serviceProgramId, vehicleId);
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(RemoveVehicleFromServiceProgram)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while removing the vehicle from the service program." });
        }
    }
}