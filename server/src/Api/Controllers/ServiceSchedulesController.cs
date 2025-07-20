using Application.Features.ServiceSchedules.Command.CreateServiceSchedule;
using Application.Features.ServiceSchedules.Command.DeleteServiceSchedule;
using Application.Features.ServiceSchedules.Command.UpdateServiceSchedule;
using Application.Features.ServiceSchedules.Query;
using Application.Features.ServiceSchedules.Query.GetAllServiceSchedule;
using Application.Features.ServiceSchedules.Query.GetServiceSchedule;
using Application.Models.PaginationModels;

using Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for <see cref="ServiceSchedule"/>
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public sealed class ServiceSchedulesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ServiceSchedulesController> _logger;

    public ServiceSchedulesController(IMediator mediator, ILogger<ServiceSchedulesController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a paginated list of all service schedules.
    /// </summary>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A paginated result containing service schedules.</returns>
    /// <response code="200">Returns the paginated list of service schedules.</response>
    /// <response code="400">Pagination parameters are invalid.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ServiceScheduleDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<ServiceScheduleDTO>>> GetServiceSchedules(
        [FromQuery] PaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetServiceSchedules)}() - Called");

            var query = new GetAllServiceScheduleQuery(parameters);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetServiceSchedules)} - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving service schedules." });
        }
    }

    /// <summary>
    /// Retrieves detailed information about a specific service schedule.
    /// </summary>
    /// <param name="id">The ID of the service schedule to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Detailed service schedule information.</returns>
    /// <response code="200">Returns the service schedule details.</response>
    /// <response code="404">Service schedule not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ServiceScheduleDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServiceScheduleDTO>> GetServiceSchedule(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetServiceSchedule)}() - Called");

            var query = new GetServiceScheduleQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetServiceSchedule)} - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the service schedule." });
        }
    }

    /// <summary>
    /// Creates a new service schedule.
    /// </summary>
    /// <param name="command">The service schedule creation command containing all required service schedule information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the newly created service schedule.</returns>
    /// <response code="201">Service schedule created successfully. Returns the service schedule ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="404">Service program not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> CreateServiceSchedule(
        [FromBody] CreateServiceScheduleCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(CreateServiceSchedule)}() - Called");

            var serviceScheduleId = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetServiceSchedule), new { id = serviceScheduleId }, serviceScheduleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(CreateServiceSchedule)} - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the service schedule." });
        }
    }

    /// <summary>
    /// Updates an existing service schedule's information.
    /// </summary>
    /// <param name="id">The ID of the service schedule to update.</param>
    /// <param name="command">The service schedule update command containing the new service schedule information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the updated service schedule.</returns>
    /// <response code="200">Service schedule updated successfully. Returns the service schedule ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="404">Service schedule or service program not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> UpdateServiceSchedule(
        int id,
        [FromBody] UpdateServiceScheduleCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(UpdateServiceSchedule)}() - Called");

            if (id != command.ServiceScheduleID) return ValidationProblem($"{nameof(UpdateServiceSchedule)} - Route ID and body ID mismatch.");

            var serviceScheduleId = await _mediator.Send(command with { ServiceScheduleID = id }, cancellationToken);

            return Ok(serviceScheduleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(UpdateServiceSchedule)} - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the service schedule." });
        }
    }

    /// <summary>
    /// Deletes a service schedule.
    /// </summary>
    /// <param name="id">The ID of the service schedule to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the deleted service schedule.</returns>
    /// <response code="200">Service schedule deleted successfully. Returns the service schedule ID.</response>
    /// <response code="404">Service schedule not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> DeleteServiceSchedule(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(DeleteServiceSchedule)}() - Called");

            var command = new DeleteServiceScheduleCommand(id);
            var serviceScheduleId = await _mediator.Send(command, cancellationToken);

            return Ok(serviceScheduleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(DeleteServiceSchedule)} - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the service schedule." });
        }
    }
}