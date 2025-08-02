using Domain.Entities;
using Domain.Entities.Enums;

using MediatR;

namespace Application.Features.Inspections.Command.CreateInspection;

/// <summary>
/// Command for creating a new <see cref="Inspection"/> performed by a technician on a vehicle.
/// </summary>
/// <param name="InspectionFormID">The ID of the <see cref="InspectionForm"/> template to use.</param>
/// <param name="VehicleID">The ID of the <see cref="Vehicle"/> being inspected.</param>
/// <param name="TechnicianID">The ID of the technician performing the inspection.</param>
/// <param name="InspectionStartTime">When the inspection started.</param>
/// <param name="InspectionEndTime">When the inspection ended.</param>
/// <param name="OdometerReading">The vehicle's odometer reading at inspection time (optional).</param>
/// <param name="VehicleCondition">The overall condition assessment of the vehicle.</param>
/// <param name="Notes">Optional notes about the inspection.</param>
/// <param name="InspectionItems">The collection of inspection item responses.</param>
/// <returns>The ID of the newly created <see cref="Inspection"/>.</returns>
/// <remarks>
/// This command creates a new inspection record with all associated inspection item responses.
/// The inspection captures a snapshot of the inspection form items at the time of inspection to preserve historical accuracy.
/// </remarks>
public record CreateInspectionCommand(
    int InspectionFormID,
    int VehicleID,
    string TechnicianID,
    DateTime InspectionStartTime,
    DateTime InspectionEndTime,
    double? OdometerReading,
    VehicleConditionEnum VehicleCondition,
    string? Notes,
    ICollection<CreateInspectionPassFailItemCommand> InspectionItems
) : IRequest<int>;

/// <summary>
/// Represents an inspection item response within a <see cref="CreateInspectionCommand"/>.
/// </summary>
/// <param name="InspectionFormItemID">The ID of the form item being responded to.</param>
/// <param name="Passed">Whether the inspection item passed or failed.</param>
/// <param name="Comment">Optional comment for this inspection item.</param>
public record CreateInspectionPassFailItemCommand(
    int InspectionFormItemID,
    bool Passed,
    string? Comment
);