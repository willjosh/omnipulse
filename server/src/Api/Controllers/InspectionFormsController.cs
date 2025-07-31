using Application.Features.InspectionForms.Command.CreateInspectionForm;

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
/// <item><b>POST /api/inspectionforms</b> - <see cref="CreateInspectionForm"/> - <see cref="CreateInspectionFormCommand"/></item>
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

    [HttpGet("{id:int}")]
    public ActionResult GetInspectionForm(int id)
    {
        return NotImplemented("GetInspectionForm endpoint not yet implemented");
    }

    private static ActionResult NotImplemented(string message)
    {
        return new ObjectResult(new { error = message })
        {
            StatusCode = StatusCodes.Status501NotImplemented
        };
    }
}