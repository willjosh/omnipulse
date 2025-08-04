using Application.Exceptions;

using Domain.Entities.Enums;

using MediatR;

namespace Application.Features.InspectionFormItems.Command.CreateInspectionFormItem;

/// <summary>
/// Command for creating a new inspection form item.
/// </summary>
/// <param name="InspectionFormID">The ID of the inspection form this item belongs to.</param>
/// <param name="ItemLabel">The label/name of the inspection item.</param>
/// <param name="ItemDescription">Optional description of what this item inspects.</param>
/// <param name="ItemInstructions">Optional instructions for how to perform this inspection.</param>
/// <param name="InspectionFormItemTypeEnum">The type of inspection item (PassFail, Numeric, Text, etc.).</param>
/// <param name="IsRequired">Whether this item is required to complete the inspection.</param>
/// <returns>The ID of the newly created inspection form item.</returns>
/// <remarks>
/// This command creates a new inspection form item that defines what should be checked
/// during an inspection. Items can be of different types (pass/fail, numeric values, text, etc.)
/// and can be marked as required or optional.
/// </remarks>
public record CreateInspectionFormItemCommand(
    int InspectionFormID,
    string ItemLabel,
    string? ItemDescription,
    string? ItemInstructions,
    InspectionFormItemTypeEnum InspectionFormItemTypeEnum = InspectionFormItemTypeEnum.PassFail, // Currently only supports Pass/Fail
    bool IsRequired = true
) : IRequest<int>;