using System;

using Application.Models;
using Application.Models.PaginationModels;

using MediatR;

namespace Application.Features.FuelPurchases.Query.GetAllFuelPurchases;

public record GetAllFuelPurchasesQuery(PaginationParameters Parameters) : IRequest<PagedResult<GetAllFuelPurchasesDTO>> { }