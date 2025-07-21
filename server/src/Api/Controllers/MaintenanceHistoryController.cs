using Application.Features.MaintenanceHistories.Query.GetAllMaintenanceHistories;
using Application.Models.PaginationModels;

using Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for <see cref="MaintenanceHistory"/>
/// </summary>
/// <remarks>
/// <b>API Endpoints</b>:
/// <list type="bullet">
/// <item><b>GET /api/maintenancehistory</b> - <see cref="GetMaintenanceHistories"/> - <see cref="GetAllMaintenanceHistoriesQuery"/></item>
/// </list>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public sealed class MaintenanceHistoryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<MaintenanceHistoryController> _logger;

    public MaintenanceHistoryController(IMediator mediator, ILogger<MaintenanceHistoryController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a paginated list of maintenance history records.
    /// </summary>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A paginated result containing maintenance history records.</returns>
    /// <response code="200">Returns the paginated list of maintenance history records.</response>
    /// <response code="400">Pagination parameters are invalid.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<GetAllMaintenanceHistoryDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<GetAllMaintenanceHistoryDTO>>> GetMaintenanceHistories(
        [FromQuery] PaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetMaintenanceHistories)}() - Called");

            var query = new GetAllMaintenanceHistoriesQuery(parameters);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetMaintenanceHistories)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving maintenance history records." });
        }
    }
}