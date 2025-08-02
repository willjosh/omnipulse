using Domain.Entities;

using MediatR;

namespace Application.Features.InspectionForms.Command.UpdateInspectionForm;

/// <summary>
/// Command for updating an existing <see cref="InspectionForm"/>.
/// </summary>
/// <param name="InspectionFormID">The ID of the inspection form to update.</param>
/// <param name="Title">The new title of the inspection form.</param>
/// <param name="Description">The new description (optional).</param>
/// <param name="IsActive">Whether the inspection form is active.</param>
/// <returns>The ID of the updated inspection form.</returns>
public record UpdateInspectionFormCommand(
    int InspectionFormID,
    string Title,
    string? Description,
    bool IsActive
) : IRequest<int>;