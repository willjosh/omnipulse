using Application.Features.Users.Command.CreateTechnician;
using Application.Features.Users.Command.DeactivateTechnician;
using Application.Features.Users.Command.UpdateTechnician;
using Application.Features.Users.Query.GetAllTechnician;
using Application.Features.Users.Query.GetTechnician;
using Application.Models.PaginationModels;

using Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for <see cref="User"/> (Technician)
/// </summary>
/// <remarks>
/// <b>API Endpoints</b>:
/// <list type="bullet">
/// <item><b>GET /api/technicians</b> - <see cref="GetTechnicians"/> - <see cref="GetAllTechnicianQuery"/></item>
/// <item><b>GET /api/technicians/{id}</b> - <see cref="GetTechnician"/> - <see cref="GetTechnicianQuery"/></item>
/// <item><b>POST /api/technicians</b> - <see cref="CreateTechnician"/> - <see cref="CreateTechnicianCommand"/></item>
/// <item><b>PUT /api/technicians/{id}</b> - <see cref="UpdateTechnician"/> - <see cref="UpdateTechnicianCommand"/></item>
/// <item><b>PATCH /api/technicians/{id}/deactivate</b> - <see cref="DeactivateTechnician"/> - <see cref="DeactivateTechnicianCommand"/></item>
/// </list>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public sealed class TechniciansController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TechniciansController> _logger;

    public TechniciansController(IMediator mediator, ILogger<TechniciansController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a paginated list of all technicians.
    /// </summary>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A paginated result containing technicians.</returns>
    /// <response code="200">Returns the paginated list of technicians.</response>
    /// <response code="400">Pagination parameters are invalid.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<GetAllTechnicianDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<GetAllTechnicianDTO>>> GetTechnicians(
        [FromQuery] PaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetTechnicians)}() - Called");

            var query = new GetAllTechnicianQuery(parameters);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Retrieves detailed information about a specific technician.
    /// </summary>
    /// <param name="id">The ID of the technician to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Detailed technician information.</returns>
    /// <response code="200">Returns the technician details.</response>
    /// <response code="404">Technician not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetTechnicianDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetTechnicianDTO>> GetTechnician(
        string id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetTechnician)}() - Called");

            var query = new GetTechnicianQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Creates a new technician.
    /// </summary>
    /// <param name="command">The technician creation command containing all required technician information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the newly created technician.</returns>
    /// <response code="201">Technician created successfully. Returns the technician ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> CreateTechnician(
        [FromBody] CreateTechnicianCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(CreateTechnician)}() - Called");

            var technicianId = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetTechnician), new { id = technicianId }, technicianId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Updates an existing technician's information.
    /// </summary>
    /// <param name="id">The ID of the technician to update.</param>
    /// <param name="command">The technician update command containing the new technician information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the updated technician.</returns>
    /// <response code="200">Technician updated successfully. Returns the technician ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="404">Technician not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> UpdateTechnician(
        string id,
        [FromBody] UpdateTechnicianCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(UpdateTechnician)}() - Called");

            if (id != command.Id) return ValidationProblem($"{nameof(UpdateTechnician)} - Route ID and body ID mismatch.");

            var technicianId = await _mediator.Send(command with { Id = id }, cancellationToken);

            return Ok(technicianId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Deactivates a technician.
    /// </summary>
    /// <param name="id">The ID of the technician to deactivate.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the deactivated technician.</returns>
    /// <response code="200">Technician deactivated successfully. Returns the technician ID.</response>
    /// <response code="404">Technician not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPatch("{id:guid}/deactivate")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> DeactivateTechnician(
        string id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(DeactivateTechnician)}() - Called");

            var command = new DeactivateTechnicianCommand(id);
            var technicianId = await _mediator.Send(command, cancellationToken);

            return Ok(technicianId);
        }
        catch (Exception)
        {
            throw;
        }
    }
}