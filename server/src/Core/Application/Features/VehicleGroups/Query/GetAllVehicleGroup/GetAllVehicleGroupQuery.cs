using Application.Models;
using Application.Models.PaginationModels;

using MediatR;

namespace Application.Features.VehicleGroups.Query.GetAllVehicleGroup;

public record GetAllVehicleGroupQuery(PaginationParameters Parameters) : IRequest<PagedResult<GetAllVehicleGroupDTO>> { }