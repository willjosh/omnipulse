using Domain.Entities;

using MediatR;

namespace Application.Features.InspectionForms.Command.DeactivateInspectionForm;

/// <summary>
/// Command to deactivate an <see cref="InspectionForm"/> by ID (soft delete).
/// </summary>
/// <param name="InspectionFormID">The ID of the <see cref="InspectionForm"/> to deactivate.</param>
public record DeactivateInspectionFormCommand(int InspectionFormID) : IRequest<int>;