using Domain.Entities;

using MediatR;

namespace Application.Features.InspectionForms.Query.GetInspectionForm;

/// <summary>
/// Query for retrieving a single <see cref="InspectionForm"/> by its ID.
/// </summary>
/// <param name="InspectionFormID">The ID of the <see cref="InspectionForm"/>.</param>
public record GetInspectionFormQuery(int InspectionFormID) : IRequest<InspectionFormDTO>;