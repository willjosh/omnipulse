using Domain.Entities;

using MediatR;

namespace Application.Features.Inspections.Query.GetInspection;

/// <summary>
/// Query for retrieving detailed information about a specific <see cref="Inspection"/> by its ID.
/// </summary>
/// <param name="InspectionID">The ID of the <see cref="Inspection"/> to retrieve.</param>
/// <returns>Detailed <see cref="Inspection"/> information including all inspection items and related data.</returns>
public record GetInspectionQuery(
    int InspectionID
) : IRequest<InspectionDTO>;