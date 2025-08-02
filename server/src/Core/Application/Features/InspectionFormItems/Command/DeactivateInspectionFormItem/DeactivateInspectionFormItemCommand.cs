using Domain.Entities;

using MediatR;

namespace Application.Features.InspectionFormItems.Command.DeactivateInspectionFormItem;

/// <summary>
/// Command to deactivate an <see cref="InspectionFormItem"/> by ID (soft delete).
/// </summary>
/// <param name="InspectionFormItemID">The ID of the <see cref="InspectionFormItem"/> to deactivate.</param>
public record DeactivateInspectionFormItemCommand(int InspectionFormItemID) : IRequest<int>;