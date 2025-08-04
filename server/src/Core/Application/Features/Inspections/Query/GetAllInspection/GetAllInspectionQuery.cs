using Application.Models.PaginationModels;

using Domain.Entities;

using MediatR;

namespace Application.Features.Inspections.Query.GetAllInspection;

/// <summary>
/// Query for retrieving a paginated list of all <see cref="Inspection"/>
/// </summary>
/// <param name="Parameters">Pagination parameters</param>
/// <returns>A paginated result containing inspection data.</returns>
public record GetAllInspectionQuery(
    PaginationParameters Parameters
) : IRequest<PagedResult<GetAllInspectionDTO>>;