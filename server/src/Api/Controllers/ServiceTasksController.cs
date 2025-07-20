using Application.Features.ServiceTasks.Command.CreateServiceTask;
using Application.Features.ServiceTasks.Command.DeleteServiceTask;
using Application.Features.ServiceTasks.Command.UpdateServiceTask;
using Application.Features.ServiceTasks.Query;
using Application.Features.ServiceTasks.Query.GetAllServiceTask;
using Application.Features.ServiceTasks.Query.GetServiceTask;
using Application.Models.PaginationModels;

using Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for <see cref="ServiceTask"/>
/// </summary>
/// <remarks>
/// <b>API Endpoints</b>:
/// <list type="bullet">
/// <item><b>GET /api/servicetasks</b> - <see cref="GetServiceTasks"/> - <see cref="GetAllServiceTaskQuery"/></item>
/// <item><b>GET /api/servicetasks/{id}</b> - <see cref="GetServiceTask"/> - <see cref="GetServiceTaskQuery"/></item>
/// <item><b>POST /api/servicetasks</b> - <see cref="CreateServiceTask"/> - <see cref="CreateServiceTaskCommand"/></item>
/// <item><b>PUT /api/servicetasks/{id}</b> - <see cref="UpdateServiceTask"/> - <see cref="UpdateServiceTaskCommand"/></item>
/// <item><b>DELETE /api/servicetasks/{id}</b> - <see cref="DeleteServiceTask"/> - <see cref="DeleteServiceTaskCommand"/></item>
/// </list>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public sealed class ServiceTasksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ServiceTasksController> _logger;

    public ServiceTasksController(IMediator mediator, ILogger<ServiceTasksController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a paginated list of all service tasks.
    /// </summary>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A paginated result containing service tasks.</returns>
    /// <response code="200">Returns the paginated list of service tasks.</response>
    /// <response code="400">Pagination parameters are invalid.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ServiceTaskDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<ServiceTaskDTO>>> GetServiceTasks(
        [FromQuery] PaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetServiceTasks)}() - Called");

            var query = new GetAllServiceTaskQuery(parameters);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetServiceTasks)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving service tasks." });
        }
    }

    /// <summary>
    /// Retrieves detailed information about a specific service task.
    /// </summary>
    /// <param name="id">The ID of the service task to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Detailed service task information.</returns>
    /// <response code="200">Returns the service task details.</response>
    /// <response code="404">Service task not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ServiceTaskDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ServiceTaskDTO>> GetServiceTask(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetServiceTask)}() - Called");

            var query = new GetServiceTaskQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetServiceTask)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the service task." });
        }
    }

    /// <summary>
    /// Creates a new service task.
    /// </summary>
    /// <param name="command">The service task creation command containing all required service task information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the newly created service task.</returns>
    /// <response code="201">Service task created successfully. Returns the service task ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="409">Service task with the same name already exists.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> CreateServiceTask(
        [FromBody] CreateServiceTaskCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(CreateServiceTask)}() - Called");

            var serviceTaskId = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetServiceTask), new { id = serviceTaskId }, serviceTaskId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(CreateServiceTask)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the service task." });
        }
    }

    /// <summary>
    /// Updates an existing service task's information.
    /// </summary>
    /// <param name="id">The ID of the service task to update.</param>
    /// <param name="command">The service task update command containing the new service task information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the updated service task.</returns>
    /// <response code="200">Service task updated successfully. Returns the service task ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="404">Service task not found.</response>
    /// <response code="409">Service task with the same name already exists.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> UpdateServiceTask(
        int id,
        [FromBody] UpdateServiceTaskCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(UpdateServiceTask)}() - Called");

            if (id != command.ServiceTaskID) return ValidationProblem($"{nameof(UpdateServiceTask)} - Route ID and body ID mismatch.");

            var serviceTaskId = await _mediator.Send(command with { ServiceTaskID = id }, cancellationToken);

            return Ok(serviceTaskId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(UpdateServiceTask)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the service task." });
        }
    }

    /// <summary>
    /// Deletes a service task.
    /// </summary>
    /// <param name="id">The ID of the service task to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the deleted service task.</returns>
    /// <response code="200">Service task deleted successfully. Returns the service task ID.</response>
    /// <response code="404">Service task not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> DeleteServiceTask(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(DeleteServiceTask)}() - Called");

            var command = new DeleteServiceTaskCommand(id);
            var serviceTaskId = await _mediator.Send(command, cancellationToken);

            return Ok(serviceTaskId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(DeleteServiceTask)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the service task." });
        }
    }
}