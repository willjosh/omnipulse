using Application.Exceptions;

using MediatR;

namespace Application.Features.InspectionForms.Command.CreateInspectionForm;

/// <summary>
/// Command for creating a new inspection form template.
/// </summary>
/// <param name="Title">The title/name of the inspection form.</param>
/// <param name="Description">Optional description of what this inspection form covers.</param>
/// <param name="IsActive">Whether this inspection form is active and available for use. Defaults to true.</param>
/// <returns>The ID of the newly created inspection form.</returns>
/// <remarks>
/// This command creates a new inspection form template that can be used to perform vehicle inspections.
/// The form itself only contains metadata - individual checklist items are added separately.
/// </remarks>
/// <exception cref="BadRequestException">
/// Thrown when validation fails for any of the input parameters.
/// </exception>
/// <exception cref="DuplicateEntityException">
/// Thrown when an inspection form with the same title already exists.
/// </exception>
public record CreateInspectionFormCommand(
    string Title,
    string? Description,
    bool IsActive = true
) : IRequest<int>;