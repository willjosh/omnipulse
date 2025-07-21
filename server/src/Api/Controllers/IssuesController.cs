using Application.Features.Issues.Command.CreateIssue;
using Application.Features.Issues.Command.DeleteIssue;
using Application.Features.Issues.Command.UpdateIssue;
using Application.Features.Issues.Query.GetAllIssue;
using Application.Features.Issues.Query.GetIssueDetails;
using Application.Models.PaginationModels;

using Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for <see cref="Issue"/>
/// </summary>
/// <remarks>
/// <b>API Endpoints</b>:
/// <list type="bullet">
/// <item><b>GET /api/issues</b> - <see cref="GetIssues"/> - <see cref="GetAllIssueQuery"/></item>
/// <item><b>GET /api/issues/{id}</b> - <see cref="GetIssue"/> - <see cref="GetIssueDetailsQuery"/></item>
/// <item><b>POST /api/issues</b> - <see cref="CreateIssue"/> - <see cref="CreateIssueCommand"/></item>
/// <item><b>PUT /api/issues/{id}</b> - <see cref="UpdateIssue"/> - <see cref="UpdateIssueCommand"/></item>
/// <item><b>DELETE /api/issues/{id}</b> - <see cref="DeleteIssue"/> - <see cref="DeleteIssueCommand"/></item>
/// </list>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public sealed class IssuesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<IssuesController> _logger;

    public IssuesController(IMediator mediator, ILogger<IssuesController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a paginated list of all issues.
    /// </summary>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A paginated result containing issues.</returns>
    /// <response code="200">Returns the paginated list of issues.</response>
    /// <response code="400">Pagination parameters are invalid.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<GetAllIssueDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<GetAllIssueDTO>>> GetIssues(
        [FromQuery] PaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetIssues)}() - Called");

            var query = new GetAllIssueQuery(parameters);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetIssues)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving issues." });
        }
    }

    /// <summary>
    /// Retrieves detailed information about a specific issue.
    /// </summary>
    /// <param name="id">The ID of the issue to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Detailed issue information.</returns>
    /// <response code="200">Returns the issue details.</response>
    /// <response code="404">Issue not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetIssueDetailsDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetIssueDetailsDTO>> GetIssue(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetIssue)}() - Called");

            var query = new GetIssueDetailsQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetIssue)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the issue." });
        }
    }

    /// <summary>
    /// Creates a new issue.
    /// </summary>
    /// <param name="command">The issue creation command containing all required issue information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the newly created issue.</returns>
    /// <response code="201">Issue created successfully. Returns the issue ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="404">Vehicle or user not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> CreateIssue(
        [FromBody] CreateIssueCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(CreateIssue)}() - Called");

            var issueId = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetIssue), new { id = issueId }, issueId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(CreateIssue)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the issue." });
        }
    }

    /// <summary>
    /// Updates an existing issue's information.
    /// </summary>
    /// <param name="id">The ID of the issue to update.</param>
    /// <param name="command">The issue update command containing the new issue information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the updated issue.</returns>
    /// <response code="200">Issue updated successfully. Returns the issue ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="404">Issue not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> UpdateIssue(
        int id,
        [FromBody] UpdateIssueCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(UpdateIssue)}() - Called");

            if (id != command.IssueID) return ValidationProblem($"{nameof(UpdateIssue)} - Route ID and body ID mismatch.");

            var issueId = await _mediator.Send(command with { IssueID = id }, cancellationToken);

            return Ok(issueId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(UpdateIssue)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the issue." });
        }
    }

    /// <summary>
    /// Deletes an issue.
    /// </summary>
    /// <param name="id">The ID of the issue to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the deleted issue.</returns>
    /// <response code="200">Issue deleted successfully. Returns the issue ID.</response>
    /// <response code="404">Issue not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> DeleteIssue(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(DeleteIssue)}() - Called");

            var command = new DeleteIssueCommand(id);
            var issueId = await _mediator.Send(command, cancellationToken);

            return Ok(issueId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(DeleteIssue)}() - ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the issue." });
        }
    }
}