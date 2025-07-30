using Application.Models.PaginationModels;

using Domain.Entities;

using MediatR;

namespace Application.Features.ServicePrograms.Query.GetAllServiceProgramVehicle;

/// <summary>
/// Query to get all vehicles assigned to a <see cref="ServiceProgram"/> with pagination.
/// </summary>
/// <param name="ServiceProgramID">The ID of the <see cref="ServiceProgram"/>.</param>
/// <param name="Parameters">Pagination parameters for the query.</param>
public record GetAllServiceProgramVehicleQuery(
    int ServiceProgramID,
    PaginationParameters Parameters
) : IRequest<PagedResult<XrefServiceProgramVehicleDTO>>;