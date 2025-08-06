using Application.Features.ServiceReminders.Command.AddServiceReminderToExistingWorkOrder;
using Application.Features.ServiceReminders.Query;
using Application.Features.ServiceReminders.Query.GetAllServiceReminders;
using Application.Models.PaginationModels;

using Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for <see cref="ServiceReminder"/>
/// </summary>
/// <remarks>
/// <b>API Endpoints</b>:
/// <list type="bullet">
/// <item><b>GET /api/servicereminders</b> - <see cref="GetAllServiceReminders"/> - <see cref="GetAllServiceRemindersQuery"/></item>
/// <item><b>PATCH /api/servicereminders/{id}</b> - <see cref="AddServiceReminderToExistingWorkOrder"/> - <see cref="AddServiceReminderToExistingWorkOrderCommand"/></item>
/// </list>
/// <b>Note</b>: ServiceReminders are calculated data generated from service schedules and vehicle assignments.
/// They do not support CREATE or DELETE operations as they are computed dynamically.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
[Authorize(Policy = "AllRoles")]
public sealed class ServiceRemindersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ServiceRemindersController> _logger;

    public ServiceRemindersController(IMediator mediator, ILogger<ServiceRemindersController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a paginated list of all calculated service reminders for all vehicles in the system.
    /// This generates multiple reminder rows for each service schedule occurrence (overdue, current, upcoming).
    /// </summary>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A paginated result containing calculated service reminders.</returns>
    /// <response code="200">Returns the paginated list of service reminders.</response>
    /// <response code="400">Pagination parameters are invalid.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ServiceReminderDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<ServiceReminderDTO>>> GetAllServiceReminders(
        [FromQuery] PaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetAllServiceReminders)}() - Called with parameters: {{@Parameters}}", parameters);

            var query = new GetAllServiceRemindersQuery(parameters);
            var result = await _mediator.Send(query, cancellationToken);

            _logger.LogInformation($"{nameof(GetAllServiceReminders)}() - Successfully retrieved {{Count}} service reminders", result.TotalCount);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetAllServiceReminders)}() - Error occurred while retrieving service reminders");
            throw;
        }
    }

    /// <summary>
    /// Links a service reminder to an existing work order.
    /// </summary>
    /// <param name="id">The ID of the service reminder to link.</param>
    /// <param name="command">The command containing the work order ID to link to.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the work order that was linked.</returns>
    /// <response code="200">Service reminder successfully linked to work order.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="404">Service reminder or work order not found.</response>
    /// <response code="409">Service reminder is already linked to a work order.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "FleetManager")]
    public async Task<ActionResult<int>> AddServiceReminderToExistingWorkOrder(
        int id,
        [FromBody] AddServiceReminderToExistingWorkOrderCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(AddServiceReminderToExistingWorkOrder)}() - Called for service reminder ID: {id}");

            if (id != command.ServiceReminderID) return BadRequest($"Route {nameof(id)} and body {nameof(command.ServiceReminderID)} mismatch");
            var result = await _mediator.Send(command with { ServiceReminderID = id }, cancellationToken);

            _logger.LogInformation($"{nameof(AddServiceReminderToExistingWorkOrder)}() - Successfully linked service reminder {id} to work order {command.WorkOrderID}");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(AddServiceReminderToExistingWorkOrder)}() - Error occurred while linking service reminder {id} to work order {command.WorkOrderID}");
            throw;
        }
    }
}