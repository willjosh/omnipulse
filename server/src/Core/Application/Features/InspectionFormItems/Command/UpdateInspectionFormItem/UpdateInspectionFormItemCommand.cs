using Domain.Entities;

using MediatR;

namespace Application.Features.InspectionFormItems.Command.UpdateInspectionFormItem;

/// <summary>
/// Command for updating an existing <see cref="InspectionFormItem"/>.
/// Notes: InspectionFormItemTypeEnum cannot be modified to maintain data integrity.
/// Items belong to one inspection form permanently.
/// </summary>
/// <param name="InspectionFormItemID">The ID of the <see cref="InspectionFormItem"/> to update.</param>
/// <param name="ItemLabel">The label/name of the inspection item.</param>
/// <param name="ItemDescription">Optional description of what this item inspects.</param>
/// <param name="ItemInstructions">Optional instructions for how to perform this inspection.</param>
/// <param name="IsRequired">Whether this item is required to complete the inspection.</param>
/// <returns>The ID of the updated <see cref="InspectionFormItem"/>.</returns>
public record UpdateInspectionFormItemCommand(
    int InspectionFormItemID,
    string ItemLabel,
    string? ItemDescription,
    string? ItemInstructions,
    bool IsRequired
) : IRequest<int>;