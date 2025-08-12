using Application.Models.PaginationModels;

using MediatR;

namespace Application.Features.FuelPurchases.Query.GetAllFuelPurchase;

public sealed record GetAllFuelPurchaseQuery(PaginationParameters Parameters) : IRequest<PagedResult<FuelPurchaseDTO>>;