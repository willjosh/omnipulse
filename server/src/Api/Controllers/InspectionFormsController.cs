using Application.Features.InspectionForms.Command.CreateInspectionForm;
using Application.Features.InspectionForms.Command.UpdateInspectionForm;
using Application.Features.InspectionForms.Query;
using Application.Features.InspectionForms.Query.GetInspectionForm;

using Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for <see cref="InspectionForm"/>
/// </summary>
/// <remarks>
/// <b>API Endpoints</b>:
/// <list type="bullet">
/// <item><b>GET /api/inspectionforms/{id}</b> - <see cref="GetInspectionForm"/> - <see cref="GetInspectionFormQuery"/></item>
/// <item><b>POST /api/inspectionforms</b> - <see cref="CreateInspectionForm"/> - <see cref="CreateInspectionFormCommand"/></item>
/// <item><b>PUT /api/inspectionforms/{id}</b> - <see cref="UpdateInspectionForm"/> - <see cref="UpdateInspectionFormCommand"/></item>
/// </list>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public sealed class InspectionFormsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<InspectionFormsController> _logger;

    public InspectionFormsController(IMediator mediator, ILogger<InspectionFormsController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves detailed information about a specific inspection form by its ID.
    /// </summary>
    /// <param name="id">The ID of the <see cref="InspectionForm"/> to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The <see cref="InspectionForm"/> details.</returns>
    /// <response code="200">Inspection form retrieved successfully.</response>
    /// <response code="404">Inspection form with the specified ID was not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(InspectionFormDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InspectionFormDTO>> GetInspectionForm(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetInspectionForm)}() - Called");

            var query = new GetInspectionFormQuery(id);
            var inspectionFormDto = await _mediator.Send(query, cancellationToken);

            return Ok(inspectionFormDto);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static ActionResult NotImplemented(string message)
    {
        return new ObjectResult(new { error = message })
        {
            StatusCode = StatusCodes.Status501NotImplemented
        };
    }

    /// <summary>
    /// Creates a new inspection form template.
    /// </summary>
    /// <param name="command">The inspection form creation command containing all required information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the newly created inspection form.</returns>
    /// <response code="201">Inspection form created successfully. Returns the inspection form ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="409">An inspection form with the same title already exists.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> CreateInspectionForm(
        [FromBody] CreateInspectionFormCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(CreateInspectionForm)}() - Called");

            var inspectionFormId = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetInspectionForm), new { id = inspectionFormId }, inspectionFormId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Updates an existing inspection form.
    /// </summary>
    /// <param name="id">The ID of the inspection form to update.</param>
    /// <param name="command">The inspection form update command containing the new information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the updated inspection form.</returns>
    /// <response code="200">Inspection form updated successfully. Returns the inspection form ID.</response>
    /// <response code="400">Request data is invalid or validation failed.</response>
    /// <response code="404">Inspection form with the specified ID was not found.</response>
    /// <response code="409">An inspection form with the same title already exists.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> UpdateInspectionForm(
        int id,
        [FromBody] UpdateInspectionFormCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(UpdateInspectionForm)}() - Called");

            if (id != command.InspectionFormID) return BadRequest($"{nameof(UpdateInspectionForm)}() - Route ID and body ID mismatch.");

            var inspectionFormId = await _mediator.Send(command with { InspectionFormID = id }, cancellationToken);

            return Ok(inspectionFormId);
        }
        catch (Exception)
        {
            throw;
        }
    }
}