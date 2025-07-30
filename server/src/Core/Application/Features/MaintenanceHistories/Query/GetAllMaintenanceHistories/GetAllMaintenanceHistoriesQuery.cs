using Application.Models.PaginationModels;

using MediatR;

namespace Application.Features.MaintenanceHistories.Query.GetAllMaintenanceHistories;

public record GetAllMaintenanceHistoriesQuery(PaginationParameters Parameters) : IRequest<PagedResult<GetAllMaintenanceHistoryDTO>> { }