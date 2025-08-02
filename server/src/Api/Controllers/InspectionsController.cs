using Application.Features.Inspections.Command.CreateInspection;
using Application.Features.Inspections.Query.GetAllInspection;
using Application.Features.Inspections.Query.GetInspection;
using Application.Models.PaginationModels;

using Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for <see cref="Inspection"/>
/// </summary>
/// <remarks>
/// <b>API Endpoints</b>:
/// <list type="bullet">
/// <item><b>GET /api/inspections</b> - <see cref="GetAllInspections"/> - <see cref="GetAllInspectionQuery"/></item>
/// <item><b>GET /api/inspections/{id}</b> - <see cref="GetInspection"/> - <see cref="GetInspectionQuery"/></item>
/// <item><b>POST /api/inspections</b> - <see cref="CreateInspection"/> - <see cref="CreateInspectionCommand"/></item>
/// </list>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public sealed class InspectionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<InspectionsController> _logger;

    public InspectionsController(IMediator mediator, ILogger<InspectionsController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a paginated list of all inspections with optional filtering and sorting.
    /// </summary>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A paginated list of inspections.</returns>
    /// <response code="200">Inspections retrieved successfully.</response>
    /// <response code="400">Request parameters are invalid or validation failed.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<GetAllInspectionDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<GetAllInspectionDTO>>> GetAllInspections(
        [FromQuery] PaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetAllInspections)}() - Called");

            var query = new GetAllInspectionQuery(parameters);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Retrieves detailed information about a specific inspection by its ID.
    /// </summary>
    /// <param name="id">The ID of the <see cref="Inspection"/> to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The <see cref="Inspection"/> details.</returns>
    /// <response code="200">Inspection retrieved successfully.</response>
    /// <response code="404">Inspection with the specified ID was not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(InspectionDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InspectionDTO>> GetInspection(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetInspection)}() - Called");

            var query = new GetInspectionQuery(id);
            var inspectionDto = await _mediator.Send(query, cancellationToken);

            return Ok(inspectionDto);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Creates a new inspection performed by a technician on a vehicle.
    /// </summary>
    /// <param name="command">The inspection creation command containing all required information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the newly created inspection.</returns>
    /// <response code="201">Inspection created successfully. Returns the inspection ID.</response>
    /// <response code="400">Request data is invalid, validation failed, or business rules are violated.</response>
    /// <response code="404">The referenced inspection form, vehicle, or technician was not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> CreateInspection(
        [FromBody] CreateInspectionCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(CreateInspection)}() - Called");

            var inspectionId = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(CreateInspection), new { id = inspectionId }, inspectionId); // TODO: Change to nameof(GetInspection)
        }
        catch (Exception)
        {
            throw;
        }
    }
}