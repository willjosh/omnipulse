using Application.Features.InspectionFormItems.Command.CreateInspectionFormItem;
using Application.Features.InspectionFormItems.Command.DeactivateInspectionFormItem;
using Application.Features.InspectionFormItems.Command.UpdateInspectionFormItem;
using Application.Features.InspectionFormItems.Query.GetAllInspectionFormItem;
using Application.Features.InspectionForms.Command.CreateInspectionForm;
using Application.Features.InspectionForms.Command.DeactivateInspectionForm;
using Application.Features.InspectionForms.Command.UpdateInspectionForm;
using Application.Features.InspectionForms.Query;
using Application.Features.InspectionForms.Query.GetAllInspectionForm;
using Application.Features.InspectionForms.Query.GetInspectionForm;
using Application.Models.PaginationModels;

using Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for <see cref="InspectionForm"/> and <see cref="InspectionFormItem"/>
/// </summary>
/// <remarks>
/// <b>API Endpoints</b>:
/// <list type="bullet">
/// <item><b>GET /api/inspectionforms</b> - <see cref="GetAllInspectionForms"/> - <see cref="GetAllInspectionFormQuery"/></item>
/// <item><b>GET /api/inspectionforms/{id}</b> - <see cref="GetInspectionForm"/> - <see cref="GetInspectionFormQuery"/></item>
/// <item><b>POST /api/inspectionforms</b> - <see cref="CreateInspectionForm"/> - <see cref="CreateInspectionFormCommand"/></item>
/// <item><b>PUT /api/inspectionforms/{id}</b> - <see cref="UpdateInspectionForm"/> - <see cref="UpdateInspectionFormCommand"/></item>
/// <item><b>PATCH /api/inspectionforms/{id}/deactivate</b> - <see cref="DeactivateInspectionForm"/> - <see cref="DeactivateInspectionFormCommand"/></item>
/// <item><b>GET /api/inspectionforms/{id}/items</b> - <see cref="GetAllInspectionFormItems"/> - <see cref="GetAllInspectionFormItemQuery"/></item>
/// <item><b>POST /api/inspectionforms/{id}/items</b> - <see cref="CreateInspectionFormItem"/> - <see cref="CreateInspectionFormItemCommand"/></item>
/// <item><b>PUT /api/inspectionforms/{id}/items/{itemId}</b> - <see cref="UpdateInspectionFormItem"/> - <see cref="UpdateInspectionFormItemCommand"/></item>
/// <item><b>PATCH /api/inspectionforms/{id}/items/{itemId}/deactivate</b> - <see cref="DeactivateInspectionFormItem"/> - <see cref="DeactivateInspectionFormItemCommand"/></item>
/// </list>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
[Authorize(Policy = "AllRoles")]
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
    /// Retrieves a paginated list of all inspection forms with optional filtering and sorting.
    /// </summary>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A paginated list of inspection forms.</returns>
    /// <response code="200">Inspection forms retrieved successfully.</response>
    /// <response code="400">Request parameters are invalid or validation failed.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<InspectionFormDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<InspectionFormDTO>>> GetAllInspectionForms(
        [FromQuery] PaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(GetAllInspectionForms)}() - Called");

            var query = new GetAllInspectionFormQuery(parameters);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception)
        {
            throw;
        }
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

    /// <summary>
    /// Deactivates an existing <see cref="InspectionForm"/> form by setting <see cref="InspectionForm.IsActive"/> to false.
    /// This preserves <see cref="Inspection"/> history while making the form unavailable for new inspections.
    /// </summary>
    /// <param name="id">The ID of the <see cref="InspectionForm"/> to deactivate.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the deactivated <see cref="InspectionForm"/>.</returns>
    /// <response code="200">Inspection form deactivated successfully. Returns the inspection form ID.</response>
    /// <response code="400">Request is invalid, inspection form is already inactive, or validation failed.</response>
    /// <response code="404">Inspection form with the specified ID was not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPatch("{id:int}/deactivate")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> DeactivateInspectionForm(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(DeactivateInspectionForm)}() - Called");

            var command = new DeactivateInspectionFormCommand(id);
            var deactivatedInspectionFormId = await _mediator.Send(command, cancellationToken);

            return Ok(deactivatedInspectionFormId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Retrieves all <see cref="InspectionFormItem"/> for a specific <see cref="InspectionForm"/>.
    /// </summary>
    /// <param name="id">The ID of the <see cref="InspectionForm"/> to get items for.</param>
    /// <param name="parameters"><see cref="PaginationParameters"/>.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A list of <see cref="InspectionFormItem"/> for the specified inspection form.</returns>
    /// <response code="200">Inspection form items retrieved successfully.</response>
    /// <response code="404">The specified inspection form was not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet("{id:int}/items")]
    [ProducesResponseType(typeof(PagedResult<GetAllInspectionFormItemDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<GetAllInspectionFormItemDTO>>> GetAllInspectionFormItems(
        int id,
        [FromQuery] PaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("{Method}() - Called with InspectionForm ID: {InspectionFormID}",
                nameof(GetAllInspectionFormItems), id);

            var query = new GetAllInspectionFormItemQuery(id, parameters);
            var inspectionFormItems = await _mediator.Send(query, cancellationToken);

            return Ok(inspectionFormItems);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Creates a new <see cref="InspectionFormItem"/> for a specific <see cref="InspectionForm"/>.
    /// </summary>
    /// <param name="id">The ID of the <see cref="InspectionForm"/> to add the <see cref="InspectionFormItem"/> to.</param>
    /// <param name="command">The inspection form item creation command containing the item details.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the newly created inspection form item.</returns>
    /// <response code="201"><see cref="InspectionFormItem"/> created successfully. Returns the <see cref="InspectionFormItem"/> ID.</response>
    /// <response code="400">Request data is invalid, validation failed, or inspection form is inactive.</response>
    /// <response code="404">The specified inspection form was not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost("{id:int}/items")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> CreateInspectionFormItem(
        int id,
        [FromBody] CreateInspectionFormItemCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(CreateInspectionFormItem)}() - Called");

            if (id != command.InspectionFormID) return BadRequest($"{nameof(CreateInspectionFormItem)}() - Route ID and body ID mismatch.");

            var inspectionFormItemId = await _mediator.Send(command with { InspectionFormID = id }, cancellationToken);

            return CreatedAtAction(nameof(GetInspectionForm), new { id = command.InspectionFormID }, inspectionFormItemId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Updates an existing <see cref="InspectionFormItem"/>.
    /// </summary>
    /// <param name="id">The ID of the <see cref="InspectionForm"/> that contains the item.</param>
    /// <param name="itemId">The ID of the <see cref="InspectionFormItem"/> to update.</param>
    /// <param name="command">The inspection form item update command containing the new details.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the updated <see cref="InspectionFormItem"/>.</returns>
    /// <response code="200">Inspection form item updated successfully. Returns the inspection form item ID.</response>
    /// <response code="400">Request data is invalid, validation failed, or inspection form is inactive.</response>
    /// <response code="404">The specified inspection form or inspection form item was not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPut("{id:int}/items/{itemId:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> UpdateInspectionFormItem(
        int id,
        int itemId,
        [FromBody] UpdateInspectionFormItemCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("{Method}() - Called with InspectionForm ID: {InspectionFormID}, Item ID: {ItemID}",
                nameof(UpdateInspectionFormItem), id, itemId);

            if (itemId != command.InspectionFormItemID)
                return BadRequest("The inspection form item ID in the URL does not match the ID in the request body.");

            var inspectionFormItemId = await _mediator.Send(command, cancellationToken);

            return Ok(inspectionFormItemId);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Deactivates an existing <see cref="InspectionFormItem"/> by setting <see cref="InspectionFormItem.IsActive"/> to false.
    /// </summary>
    /// <param name="id">The ID of the <see cref="InspectionForm"/> that contains the item.</param>
    /// <param name="itemId">The ID of the <see cref="InspectionFormItem"/> to deactivate.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the deactivated <see cref="InspectionFormItem"/>.</returns>
    /// <response code="200">Inspection form item deactivated successfully. Returns the inspection form item ID.</response>
    /// <response code="400">Request is invalid, inspection form item is already inactive, or validation failed.</response>
    /// <response code="404">Inspection form item with the specified ID was not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPatch("{id:int}/items/{itemId:int}/deactivate")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> DeactivateInspectionFormItem(
        int id,
        int itemId,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("{Method}() - Called with InspectionForm ID: {InspectionFormID}, Item ID: {ItemID}",
                nameof(DeactivateInspectionFormItem), id, itemId);

            var command = new DeactivateInspectionFormItemCommand(itemId);
            var deactivatedInspectionFormItemId = await _mediator.Send(command, cancellationToken);

            return Ok(deactivatedInspectionFormItemId);
        }
        catch (Exception)
        {
            throw;
        }
    }
}