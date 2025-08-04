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
    [ProducesResponseType(typeof(PagedResult<GetAllServiceReminderDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<GetAllServiceReminderDTO>>> GetAllServiceReminders(
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
}